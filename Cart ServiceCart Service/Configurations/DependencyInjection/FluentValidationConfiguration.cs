using FluentValidation;
using System.Reflection;
namespace Cart_ServiceCart_Service.Configurations.DependencyInjection;

public static class FluentValidationConfiguration
{
    public static IServiceCollection AddFluentValidationConfiguration(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
