using Catalog_Service.Common.BaseHandler;
using Catalog_Service.Common.Middelwares;
using Catalog_Service.Common.Services;
using Catalog_Service.Common.Settings;

namespace Catalog_Service.Configurations.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();
            services.AddHttpContextAccessor();
            // One UserState per request, surfaced through two interfaces: everything reads via
            // IUserState, only UserStateMiddleware writes via IUserStateWriter. Both delegate to
            // the same scoped instance — resolving them separately would hand the middleware a
            // different object than the handlers read from.
            services.AddScoped<UserState>();
            services.AddScoped<IUserState>(sp => sp.GetRequiredService<UserState>());
            services.AddScoped<IUserStateWriter>(sp => sp.GetRequiredService<UserState>());
            services.AddScoped<UserStateMiddleware>();
            services.AddScoped<CancellationTokenCapture>();   // populated per request by CancellationTokenCaptureMiddleware
            services.AddScoped<CancellationTokenCaptureMiddleware>();
            services.AddScoped<GlobalErrorHandlerMiddleware>();
            services.AddScoped<ValidationExceptionHandlingMiddleware>();
            services.AddScoped<BaseRequestParameters>();

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
