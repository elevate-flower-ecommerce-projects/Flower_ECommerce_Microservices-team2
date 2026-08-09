using AuthService.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Configurations.DependencyInjection
{
    public static class ContextConfiguration
    {
        public static IServiceCollection AddDBContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AuthDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("AuthDb")));

            return services;
        }
    }
}
