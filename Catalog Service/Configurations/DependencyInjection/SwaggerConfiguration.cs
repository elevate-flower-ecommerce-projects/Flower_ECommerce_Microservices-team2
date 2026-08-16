using Microsoft.OpenApi;

namespace Catalog_Service.Configurations.DependencyInjection
{
    public static class SwaggerConfiguration
    {
        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Catalog Service API",
                    Version = "v1",
                    Description = "Product Catalog Management Microservice"
                });

                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "Enter your JWT token.",
                    In = ParameterLocation.Header
                });

                // Microsoft.OpenApi 2.x replaced the inline `Reference = new OpenApiReference { ... }`
                // pattern with dedicated reference types, and Swashbuckle 10 takes a factory so the
                // reference can be resolved against the host document.
                opt.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecuritySchemeReference("Bearer", document),
                        new List<string>()
                    }
                });
            });

            return services;
        }


    }
}
