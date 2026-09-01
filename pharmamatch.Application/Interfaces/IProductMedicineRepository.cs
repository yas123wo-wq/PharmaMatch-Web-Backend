using pharmamatch.Domain.Entities;

namespace pharmamatch.Application.Interfaces
{
    /// <summary>
    /// واجهة مستودع بيانات الأدوية (ProductMedicine) في طبقة التطبيق.
    /// تحدد العمليات الأساسية دون التعامل المباشر مع قاعدة البيانات.
    /// </summary>
    public interface IProductMedicineRepository
    {
        Task<IEnumerable<ProductMedicine>> GetAllAsync();
        Task<ProductMedicine?> GetByIdAsync(int id);
        Task<ProductMedicine> CreateAsync(ProductMedicine medicine);
        Task<ProductMedicine> UpdateAsync(ProductMedicine medicine);
        Task DeleteAsync(int id);
        bool Exists(int id);

        /// <summary>
        /// استعلام البحث الذكي عن الأدوية البديلة التي تشترك في نفس المادة الفعالة
        /// </summary>
        Task<IEnumerable<ProductMedicine>> GetAlternativesAsync(int medicineId);

        /// <summary>
        /// تبديل حالة المتابعة للدواء
        /// </summary>
        Task ToggleWatchlistAsync(int id);

        /// <summary>
        /// جلب أدوية قائمة المتابعة مع الفلترة الاختيارية
        /// </summary>
        Task<IEnumerable<ProductMedicine>> GetWatchlistAsync(string? search = null);

        /// <summary>
        /// البحث المتقدم والفلترة للأدوية
        /// </summary>
        Task<IEnumerable<ProductMedicine>> SearchMedicinesAsync(string? query, int? categoryId, string? statusFilter);

        /// <summary>
        /// جلب البدائل التفاعلية لبيانات البحث
        /// </summary>
        Task<IEnumerable<ProductMedicine>> GetAlternativesForIngredientsAsync(IEnumerable<int> ingredientIds, IEnumerable<int> excludeMedicineIds);

        /// <summary>
        /// حساب عدد الأصناف الحرجة
        /// </summary>
        Task<int> GetCriticalItemsCountAsync();

        /// <summary>
        /// إضافة دواء جديد مع دفعة مخزون أولية
        /// </summary>
        Task AddWithBatchAsync(ProductMedicine medicine, InventoryBatch batch);

        /// <summary>
        /// تحديث بيانات دواء ودفعة المخزون الخاصة به
        /// </summary>
        Task UpdateWithBatchAsync(ProductMedicine medicine, int totalQuantity, string? batchNumber, DateTime expiryDate);
    }
}
