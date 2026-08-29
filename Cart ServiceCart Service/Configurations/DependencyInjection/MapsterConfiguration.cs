using Mapster;
using MapsterMapper;
using System.Reflection;
namespace Cart_ServiceCart_Service.Configurations.DependencyInjection;

public static class MapsterConfiguration
{
    public static IServiceCollection AddMapsterConfiguration(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        // Scans the current assembly for any implementations of IRegister
        config.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton(config);
        services.AddTransient<IMapper, ServiceMapper>();

        return services;
    }
}
