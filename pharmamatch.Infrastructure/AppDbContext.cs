using Microsoft.EntityFrameworkCore;
using pharmamatch.Domain.Entities;

namespace pharmamatch.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // ربط الجداول الأربعة بقاعدة البيانات
        public DbSet<Category> Categories { get; set; }
        public DbSet<ActiveIngredient> ActiveIngredients { get; set; }
        public DbSet<ProductMedicine> Medicines { get; set; }
        public DbSet<InventoryBatch> InventoryBatches { get; set; }
    }
}
