using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Smart_Freelance_Core.Behaviors;
using System.Reflection;
namespace Smart_Freelance_Core
{
    public static class ModuleCoreDependencies
    {
        public static IServiceCollection AddCoreDependencies(this IServiceCollection services)
        {
            //Configuration Of Mediator
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

            // Get Validators
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            // 
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuditLoggingBehavior<,>));


            return services;
        }
    }
}
