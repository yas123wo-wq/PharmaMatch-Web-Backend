using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using pharmamatch.Application.Interfaces;
using pharmamatch.Infrastructure.Data;
using pharmamatch.Infrastructure.Repositories;

namespace pharmamatch.Infrastructure
{
    /// <summary>
    /// صنف تسجيل التبعيات لطبقة الـ Infrastructure.
    /// يربط الـ DbContext والـ Repositories بحاوية حقن التبعيات (DI Container).
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrEmpty(connectionString))
            {
                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(connectionString));
            }

            services.AddScoped<IProductMedicineRepository, ProductMedicineRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IActiveIngredientRepository, ActiveIngredientRepository>();
            services.AddScoped<IInventoryBatchRepository, InventoryBatchRepository>();

            return services;
        }

        /// <summary>
        /// تهيئة وتعبئة قاعدة البيانات تلقائياً دون الحاجة لتضمين EF Core في طبقة العرض
        /// </summary>
        public static void SeedDatabase(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.Migrate();
            DbSeeder.Seed(context);
        }
    }
}
