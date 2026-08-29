using Cart_ServiceCart_Service.Common.Behaviors;
namespace Cart_ServiceCart_Service.Configurations.DependencyInjection;

public static class MediatRConfiguration
{
    public static IServiceCollection AddMediatRConfiguration(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
            cfg.AddOpenBehavior(typeof(TransactionMiddleware<,>));
        });

        return services;
    }
}
