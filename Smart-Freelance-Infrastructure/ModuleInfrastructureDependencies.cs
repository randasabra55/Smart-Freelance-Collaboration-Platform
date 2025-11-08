using Microsoft.Extensions.DependencyInjection;
using Smart_Freelance_Infrastructure.Common.Pagination.Service;
using Smart_Freelance_Infrastructure.InrastuctureBases;
using Smart_Freelance_Infrastructure.Services;

namespace Smart_Freelance_Infrastructure
{
    public static class ModuleInfrastructureDependencies
    {
        public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
        {
            services.AddTransient<AuditLoggerService>();

            services.AddTransient<IPaginationService, PaginationService>();

            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            return services;
        }
    }
}
