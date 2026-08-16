using AuthService.Common.BaseHandler;
using AuthService.Common.Middelwares;
using AuthService.Common.Services;
using AuthService.Common.Settings;

namespace AuthService.Configurations.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();
            services.AddHttpContextAccessor(); // Required for CurrentUserService to work!
            services.AddScoped<CurrentUserService>();
            services.AddScoped<GlobalErrorHandlerMiddleware>();
            services.AddScoped<ValidationExceptionHandlingMiddleware>();
            services.AddScoped<BaseParameters>();

            // Email service (MailKit)
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.AddScoped<IEmailService, EmailService>();

            // Register the standard IdGen Snowflake ID Generator (originally developed by Twitter)
            // Generator ID 0 is used as the machine/worker ID — change per node to avoid collisions.
            services.AddSingleton<IdGen.IIdGenerator<long>>(x => new IdGen.IdGenerator(0));

            return services;
        }
    }
}
