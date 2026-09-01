using pharmamatch.Domain.Entities;

namespace pharmamatch.Application.Interfaces
{
    public interface IInventoryBatchRepository
    {
        Task<IEnumerable<InventoryBatch>> GetAllAsync();
        Task<InventoryBatch?> GetByIdAsync(int id);
        Task<InventoryBatch> CreateAsync(InventoryBatch batch);
        Task<InventoryBatch> UpdateAsync(InventoryBatch batch);
        Task DeleteAsync(int id);
        bool Exists(int id);

        /// <summary>
        /// جلب إجمالي كمية المخزون عبر جميع الشحنات
        /// </summary>
        Task<int> GetTotalStockAsync();

        /// <summary>
        /// جلب الشحنات منتهية الصلاحية
        /// </summary>
        Task<IEnumerable<InventoryBatch>> GetExpiredBatchesAsync();

        /// <summary>
        /// جلب عدد الشحنات منتهية الصلاحية
        /// </summary>
        Task<int> GetExpiredCountAsync();
    }
}
