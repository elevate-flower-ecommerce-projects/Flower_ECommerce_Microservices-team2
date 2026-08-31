using Cart_ServiceCart_Service.Common.BaseHandler;
using Cart_ServiceCart_Service.Common.Interface;
using Cart_ServiceCart_Service.Common.Middelwares;
using Cart_ServiceCart_Service.Common.Repositories;
using Cart_ServiceCart_Service.Common.Services;
namespace Cart_ServiceCart_Service.Configurations.DependencyInjection;

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
        services.AddScoped<BaseParameters>();

        // Generic repository exposing IQueryable table access and pagination.
        // Injected automatically via BaseParameters into every handler.
        services.AddScoped<IGenericRepository, GenericRepository>();


        // Register the standard IdGen Snowflake ID Generator (originally developed by Twitter)
        // Generator ID 0 is used as the machine/worker ID — change per node to avoid collisions.
        services.AddSingleton<IdGen.IIdGenerator<long>>(x => new IdGen.IdGenerator(0));

        return services;
    }
}
