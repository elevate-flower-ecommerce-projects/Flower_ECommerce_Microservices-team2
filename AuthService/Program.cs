using AuthService.Common.Middelwares;
using AuthService.Configurations.DependencyInjection;
using AuthService.Data.Seeding;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────────────────────────────────
builder.Services.AddDBContext(builder.Configuration);                    // EF Core (AuthDbContext)
builder.Services.AddApplicationServices(builder.Configuration);          // CurrentUserService, EmailService, IdGen, Cache
builder.Services.AddMediatRConfiguration();                              // MediatR + Validation + Transaction behaviors
builder.Services.AddFluentValidationConfiguration();                     // FluentValidation validators
builder.Services.AddMapsterConfiguration();                              // Mapster object mapping
builder.Services.AddAuthenticationConfiguration(builder.Configuration); // JWT Bearer auth
builder.Services.AddCapConfiguration(builder.Configuration);            // CAP (outbox) + RabbitMQ
builder.Services.AddSwaggerConfiguration();                              // Swagger + JWT Bearer security definition

// ── MVC ───────────────────────────────────────────────────────────────────────
builder.Services.AddControllers();
//
var app = builder.Build();

await AuthDataSeeder.SeedAsync(app.Services);

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthService API v1");
});

app.MapGet("/", () => Results.Redirect("/swagger"));

// ── HTTP Pipeline ─────────────────────────────────────────────────────────────
app.UseHttpsRedirection();

// Global error handling middleware (outermost — catches all unhandled exceptions)
app.UseMiddleware<ValidationExceptionHandlingMiddleware>();
app.UseMiddleware<GlobalErrorHandlerMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "Healthy", service = "AuthService", timestamp = DateTime.UtcNow }));

app.Run();
