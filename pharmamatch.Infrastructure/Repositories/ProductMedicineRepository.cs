using Microsoft.EntityFrameworkCore;
using pharmamatch.Application.Interfaces;
using pharmamatch.Domain.Entities;

namespace pharmamatch.Infrastructure.Repositories
{
    /// <summary>
    /// تنفيذ مستودع بيانات الأدوية (ProductMedicineRepository) في طبقة الـ Infrastructure.
    /// هذه الطبقة هي الجهة الوحيدة التي تتصل بقاعدة البيانات وتستعين بـ AppDbContext.
    /// </summary>
    public class ProductMedicineRepository : IProductMedicineRepository
    {
        private readonly AppDbContext _context;

        public ProductMedicineRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductMedicine>> GetAllAsync()
        {
            return await _context.Medicines
                .Include(m => m.ActiveIngredient)
                    .ThenInclude(a => a!.Category)
                .Include(m => m.InventoryBatches)
                .ToListAsync();
        }

        public async Task<ProductMedicine?> GetByIdAsync(int id)
        {
            return await _context.Medicines
                .Include(m => m.ActiveIngredient)
                    .ThenInclude(a => a!.Category)
                .Include(m => m.InventoryBatches)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<ProductMedicine> CreateAsync(ProductMedicine medicine)
        {
            _context.Medicines.Add(medicine);
            await _context.SaveChangesAsync();
            return medicine;
        }

        public async Task<ProductMedicine> UpdateAsync(ProductMedicine medicine)
        {
            _context.Entry(medicine).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return medicine;
        }

        public async Task DeleteAsync(int id)
        {
            var medicine = await _context.Medicines
                .Include(m => m.InventoryBatches)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medicine != null)
            {
                _context.InventoryBatches.RemoveRange(medicine.InventoryBatches);
                _context.Medicines.Remove(medicine);
                await _context.SaveChangesAsync();
            }
        }

        public bool Exists(int id)
        {
            return _context.Medicines.Any(e => e.Id == id);
        }

        /// <summary>
        /// جلب الأدوية البديلة التي تشترك في نفس المادة الفعالة
        /// </summary>
        public async Task<IEnumerable<ProductMedicine>> GetAlternativesAsync(int medicineId)
        {
            var targetMedicine = await _context.Medicines.FindAsync(medicineId);
            if (targetMedicine == null)
            {
                return Enumerable.Empty<ProductMedicine>();
            }

            return await _context.Medicines
                .Include(m => m.ActiveIngredient)
                    .ThenInclude(a => a!.Category)
                .Include(m => m.InventoryBatches)
                .Where(m => m.IngredientId == targetMedicine.IngredientId && m.Id != medicineId)
                .ToListAsync();
        }

        public async Task ToggleWatchlistAsync(int id)
        {
            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine != null)
            {
                medicine.IsWatchlist = !medicine.IsWatchlist;
                medicine.LastUpdated = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ProductMedicine>> GetWatchlistAsync(string? search = null)
        {
            var query = _context.Medicines
                .Include(m => m.ActiveIngredient)
                    .ThenInclude(a => a!.Category)
                .Include(m => m.InventoryBatches)
                .Where(m => m.IsWatchlist);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(m => m.TradeName.Contains(search) ||
                                         (m.ActiveIngredient != null && m.ActiveIngredient.ScientificName.Contains(search)));
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<ProductMedicine>> SearchMedicinesAsync(string? query, int? categoryId, string? statusFilter)
        {
            var medicinesQuery = _context.Medicines
                .Include(m => m.ActiveIngredient)
                    .ThenInclude(a => a!.Category)
                .Include(m => m.InventoryBatches)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                medicinesQuery = medicinesQuery.Where(m =>
                    m.TradeName.Contains(query) ||
                    (m.ActiveIngredient != null && m.ActiveIngredient.ScientificName.Contains(query))
                );
            }

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                medicinesQuery = medicinesQuery.Where(m => m.ActiveIngredient != null && m.ActiveIngredient.CategoryId == categoryId.Value);
            }

            if (statusFilter == "available")
            {
                medicinesQuery = medicinesQuery.Where(m => m.InventoryBatches.Sum(b => b.Quantity) > 50);
            }
            else if (statusFilter == "low")
            {
                medicinesQuery = medicinesQuery.Where(m => m.InventoryBatches.Sum(b => b.Quantity) <= 50);
            }

            return await medicinesQuery.ToListAsync();
        }

        public async Task<IEnumerable<ProductMedicine>> GetAlternativesForIngredientsAsync(IEnumerable<int> ingredientIds, IEnumerable<int> excludeMedicineIds)
        {
            if (!ingredientIds.Any()) return Enumerable.Empty<ProductMedicine>();

            return await _context.Medicines
                .Include(m => m.ActiveIngredient)
                    .ThenInclude(a => a!.Category)
                .Include(m => m.InventoryBatches)
                .Where(m => ingredientIds.Contains(m.IngredientId) && !excludeMedicineIds.Contains(m.Id))
                .ToListAsync();
        }

        public async Task<int> GetCriticalItemsCountAsync()
        {
            return await _context.Medicines.CountAsync(m =>
                m.InventoryBatches.Sum(b => b.Quantity) <= 50 ||
                m.InventoryBatches.Any(b => b.ExpiryDate < DateTime.Now)
            );
        }

        public async Task AddWithBatchAsync(ProductMedicine medicine, InventoryBatch batch)
        {
            _context.Medicines.Add(medicine);
            await _context.SaveChangesAsync();

            batch.MedicineId = medicine.Id;
            _context.InventoryBatches.Add(batch);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateWithBatchAsync(ProductMedicine medicine, int totalQuantity, string? batchNumber, DateTime expiryDate)
        {
            var existingMedicine = await _context.Medicines
                .Include(m => m.InventoryBatches)
                .FirstOrDefaultAsync(m => m.Id == medicine.Id);

            if (existingMedicine != null)
            {
                existingMedicine.TradeName = medicine.TradeName;
                existingMedicine.Price = medicine.Price;
                existingMedicine.IngredientId = medicine.IngredientId;
                existingMedicine.LastUpdated = DateTime.Now;

                var batch = existingMedicine.InventoryBatches.FirstOrDefault();
                if (batch != null)
                {
                    batch.Quantity = totalQuantity;
                    if (!string.IsNullOrWhiteSpace(batchNumber)) batch.BatchNumber = batchNumber.Trim();
                    if (expiryDate > DateTime.MinValue) batch.ExpiryDate = expiryDate;
                }
                else
                {
                    _context.InventoryBatches.Add(new InventoryBatch
                    {
                        MedicineId = existingMedicine.Id,
                        BatchNumber = string.IsNullOrWhiteSpace(batchNumber) ? $"BN-EDIT-{existingMedicine.Id}" : batchNumber.Trim(),
                        Quantity = totalQuantity,
                        ExpiryDate = expiryDate > DateTime.MinValue ? expiryDate : DateTime.Now.AddYears(1)
                    });
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
