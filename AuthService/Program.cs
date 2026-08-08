using AuthService.Common.Middelwares;
using AuthService.Configurations.DependencyInjection;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────────────────────────────────
builder.Services.AddDBContext(builder.Configuration);                    // EF Core (AuthDbContext)
builder.Services.AddApplicationServices(builder.Configuration);          // CurrentUserService, EmailService, IdGen, Cache
builder.Services.AddHangfireConfiguration(builder.Configuration);        // Hangfire background jobs + SQL Server storage
builder.Services.AddMediatRConfiguration();                              // MediatR + Validation + Transaction behaviors
builder.Services.AddFluentValidationConfiguration();                     // FluentValidation validators
builder.Services.AddMapsterConfiguration();                              // Mapster object mapping
builder.Services.AddAuthenticationConfiguration(builder.Configuration); // JWT Bearer auth
builder.Services.AddCapConfiguration(builder.Configuration);            // CAP (outbox) + RabbitMQ
builder.Services.AddSwaggerConfiguration();                              // OpenAPI spec + Swagger UI registration

// ── MVC ───────────────────────────────────────────────────────────────────────
builder.Services.AddControllers();

var app = builder.Build();

// ── Swagger / OpenAPI Middleware ───────────────────────────────────────────────
app.UseSwaggerConfiguration();

// ── Hangfire Dashboard (available at /hangfire) ───────────────────────────────
app.UseHangfireDashboard("/hangfire");

// ── HTTP Pipeline ─────────────────────────────────────────────────────────────
app.UseHttpsRedirection();

// Global error handling middleware (outermost — catches all unhandled exceptions)
app.UseMiddleware<ValidationExceptionHandlingMiddleware>();
app.UseMiddleware<GlobalErrorHandlerMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
