using Address___Store_Coverage_Service.Data;
using Microsoft.EntityFrameworkCore;

namespace Address___Store_Coverage_Service.Configurations.DependencyInjection;

public static class ContextConfiguration
{
    public static IServiceCollection AddDBContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AddressStoreCoverageDb")
            ?? configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AddressStoreCoverageDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }
}
