using Catalog_Service.Data;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.Configurations.DependencyInjection
{
    public static class ContextConfiguration
    {
        public static IServiceCollection AddDBContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CatalogServiceDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("CatalogServiceDb")));

            return services;
        }
    }
}
