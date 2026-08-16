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
builder.Services.AddControllers();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog Service API v1");
});

app.MapGet("/", () => Results.Redirect("/swagger"));

app.UseHttpsRedirection();

// Captures HttpContext.RequestAborted into the scoped CancellationTokenCapture.
// Registered first so the token is available to everything below it.
app.UseMiddleware<CancellationTokenCaptureMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

// Projects the JWT claims onto the scoped UserState — must come after UseAuthentication().
app.UseMiddleware<UserStateMiddleware>();

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "Healthy", service = "Catalog Service", timestamp = DateTime.UtcNow }));

app.Run();

