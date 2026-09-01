using Microsoft.Extensions.DependencyInjection;

namespace pharmamatch.Application
{
    /// <summary>
    /// صنف تسجيل التبعيات لطبقة الـ Application.
    /// يقوم بتسجيل خدمات MediatR وتلقين المعالجات (Handlers).
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
            return services;
        }
    }
}
