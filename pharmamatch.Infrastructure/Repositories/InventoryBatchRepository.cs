using Microsoft.EntityFrameworkCore;
using pharmamatch.Application.Interfaces;
using pharmamatch.Domain.Entities;

namespace pharmamatch.Infrastructure.Repositories
{
    public class InventoryBatchRepository : IInventoryBatchRepository
    {
        private readonly AppDbContext _context;

        public InventoryBatchRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InventoryBatch>> GetAllAsync()
        {
            return await _context.InventoryBatches
                .Include(b => b.ProductMedicine)
                .ToListAsync();
        }

        public async Task<InventoryBatch?> GetByIdAsync(int id)
        {
            return await _context.InventoryBatches
                .Include(b => b.ProductMedicine)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<InventoryBatch> CreateAsync(InventoryBatch batch)
        {
            _context.InventoryBatches.Add(batch);
            await _context.SaveChangesAsync();
            return batch;
        }

        public async Task<InventoryBatch> UpdateAsync(InventoryBatch batch)
        {
            _context.Entry(batch).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return batch;
        }

        public async Task DeleteAsync(int id)
        {
            var batch = await _context.InventoryBatches.FindAsync(id);
            if (batch != null)
            {
                _context.InventoryBatches.Remove(batch);
                await _context.SaveChangesAsync();
            }
        }

        public bool Exists(int id)
        {
            return _context.InventoryBatches.Any(e => e.Id == id);
        }

        public async Task<int> GetTotalStockAsync()
        {
            return await _context.InventoryBatches.SumAsync(b => (int?)b.Quantity) ?? 0;
        }

        public async Task<IEnumerable<InventoryBatch>> GetExpiredBatchesAsync()
        {
            return await _context.InventoryBatches
                .Include(b => b.ProductMedicine)
                .Where(b => b.ExpiryDate < DateTime.Now)
                .ToListAsync();
        }

        public async Task<int> GetExpiredCountAsync()
        {
            return await _context.InventoryBatches.CountAsync(b => b.ExpiryDate < DateTime.Now);
        }
    }
}
