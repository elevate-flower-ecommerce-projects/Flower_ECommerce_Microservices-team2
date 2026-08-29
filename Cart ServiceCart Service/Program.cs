using Cart_ServiceCart_Service.Common.Middelwares;
using Cart_ServiceCart_Service.Configurations.DependencyInjection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────────────────────────────────
builder.Services.AddApplicationServices(builder.Configuration);          // UserState, IdGen, Cache
builder.Services.AddMediatRConfiguration();                              // MediatR + Validation + Transaction behaviors
builder.Services.AddFluentValidationConfiguration();                     // FluentValidation validators
builder.Services.AddMapsterConfiguration();                              // Mapster object mapping
builder.Services.AddAuthenticationConfiguration(builder.Configuration); // JWT Bearer auth
builder.Services.AddCapConfiguration(builder.Configuration);            // CAP (outbox) + RabbitMQ
//builder.Services.AddSwaggerConfiguration();                           // Swagger + JWT Bearer security definition
builder.Services.AddControllers();
// Add services to the container.
builder.Services.AddDBContext(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Cart API",
        Version = "v1"
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cart API v1");
});

app.UseHttpsRedirection();

// Global error handling middleware
app.UseMiddleware<ValidationExceptionHandlingMiddleware>();
app.UseMiddleware<GlobalErrorHandlerMiddleware>();

// Captures HttpContext.RequestAborted into the scoped CancellationTokenCapture.
// Registered first so the token is available to everything below it.
app.UseMiddleware<CancellationTokenCaptureMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

// Projects the JWT claims onto the scoped UserState — must come after UseAuthentication().
app.UseMiddleware<UserStateMiddleware>();

app.MapControllers();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapGet("/health", () => Results.Ok(new { status = "Healthy", service = "Cart Service", timestamp = DateTime.UtcNow }));

app.Run();

