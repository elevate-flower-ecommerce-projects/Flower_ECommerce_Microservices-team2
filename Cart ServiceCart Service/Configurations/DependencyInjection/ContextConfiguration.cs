using Cart_ServiceCart_Service.Data;
using Microsoft.EntityFrameworkCore;

namespace Cart_ServiceCart_Service.Configurations.DependencyInjection;

public static class ContextConfiguration
{
    public static IServiceCollection AddDBContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("CartDb")
            ?? configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<CartDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }
}
