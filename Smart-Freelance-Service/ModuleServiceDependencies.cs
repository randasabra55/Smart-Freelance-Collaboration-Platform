using Microsoft.Extensions.DependencyInjection;
using Smart_Freelance_Service.Abstracts;
using Smart_Freelance_Service.Implementations;

namespace Smart_Freelance_Service
{
    public static class ModuleServiceDependencies
    {
        public static IServiceCollection AddServiceDependencies(this IServiceCollection services)
        {
            // services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ICookieManager, CookieManager>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddSingleton<IEmailSender, EmailSender>();
            services.AddScoped<IEmailConfirmationService, EmailConfirmationService>();
            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddScoped<ITokenValidator, TokenValidator>();
            services.AddScoped<ITokensService, TokensService>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped(typeof(Lazy<>));
            // services.AddScoped<PaymentService>();


            services.AddHttpContextAccessor();

            return services;
        }
    }
}
