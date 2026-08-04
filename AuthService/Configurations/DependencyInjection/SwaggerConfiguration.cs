namespace AuthService.Configurations.DependencyInjection
{
    public static class SwaggerConfiguration
    {
        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            // ── OpenAPI spec (.NET 10 native) ─────────────────────────────────────
            // Generates the raw JSON spec at /openapi/v1.json
            services.AddOpenApi("v1", options =>
            {
                options.AddDocumentTransformer((document, context, _) =>
                {
                    document.Info = new()
                    {
                        Title       = "Auth Service API",
                        Version     = "v1",
                        Description = "Handles authentication, authorization, user management, and token operations."
                    };
                    return Task.CompletedTask;
                });
            });

            // ── Swashbuckle UI ─────────────────────────────────────────────────────
            services.AddEndpointsApiExplorer();

            return services;
        }

        public static WebApplication UseSwaggerConfiguration(this WebApplication app)
        {
            // Enabled in Development and Docker environments so the UI is reachable
            if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
            {
                app.MapOpenApi(); // → /openapi/v1.json (native .NET 10)

                app.UseSwaggerUI(ui =>
                {
                    ui.SwaggerEndpoint("/openapi/v1.json", "Auth Service v1");
                    ui.RoutePrefix           = "swagger";       // → http://localhost:5000/swagger
                    ui.DocumentTitle         = "Auth Service API";
                    ui.DisplayRequestDuration();                // shows each request's execution time in ms
                    ui.EnableTryItOutByDefault();               // "Try it out" open by default
                });
            }

            return app;
        }
    }
}
