using Catalog_Service.Common.Middelwares;
using Catalog_Service.Configurations.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────────────────────────────────
builder.Services.AddDBContext(builder.Configuration);                    // EF Core (AuthDbContext)
builder.Services.AddApplicationServices(builder.Configuration);          // UserState, EmailService, IdGen, Cache
builder.Services.AddMediatRConfiguration();                              // MediatR + Validation + Transaction behaviors
builder.Services.AddFluentValidationConfiguration();                     // FluentValidation validators
builder.Services.AddMapsterConfiguration();                              // Mapster object mapping
builder.Services.AddAuthenticationConfiguration(builder.Configuration); // JWT Bearer auth
builder.Services.AddCapConfiguration(builder.Configuration);            // CAP (outbox) + RabbitMQ
builder.Services.AddSwaggerConfiguration();                              // Swagger + JWT Bearer security definition

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog Service API v1");
});

app.UseHttpsRedirection();

// Captures HttpContext.RequestAborted into the scoped CancellationTokenCapture.
// Registered first so the token is available to everything below it.
app.UseMiddleware<CancellationTokenCaptureMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

// Projects the JWT claims onto the scoped UserState — must come after UseAuthentication().
app.UseMiddleware<UserStateMiddleware>();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/health", () => Results.Ok(new { status = "Healthy", service = "Catalog Service", timestamp = DateTime.UtcNow }));

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
