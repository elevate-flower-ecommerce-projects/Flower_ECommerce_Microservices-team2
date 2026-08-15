using Catalog_Service.Data;

namespace Catalog_Service.Configurations.DependencyInjection
{
    public static class CapConfiguration
    {
        public static IServiceCollection AddCapConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCap(x =>
            {
                // Register Entity Framework Core to store CAP messages (outbox pattern)
                x.UseEntityFramework<CatalogServiceDbContext>();

                // Configure SQL Server (matching the Catalog_Service DB setup)
                x.UseSqlServer(configuration.GetConnectionString("AuthDb")!);

                // Configure RabbitMQ as the message broker
                x.UseRabbitMQ(options =>
                {
                    options.HostName = configuration["RabbitMQ:HostName"] ?? "localhost";
                    options.UserName = configuration["RabbitMQ:UserName"] ?? "guest";
                    options.Password = configuration["RabbitMQ:Password"] ?? "guest";
                    // Default port is 5672
                });
            });

            return services;
        }
    }
}
