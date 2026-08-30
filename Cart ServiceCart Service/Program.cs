using Cart_ServiceCart_Service.Common.BaseHandler;
using Cart_ServiceCart_Service.Common.Middelwares;
using Cart_ServiceCart_Service.Configurations.DependencyInjection;
using Cart_ServiceCart_Service.Features.Cart;
using Cart_ServiceCart_Service.Features.Cart.AddItem;
using Cart_ServiceCart_Service.Features.Cart.GetProductAvailability;
using Cart_ServiceCart_Service.Features.Cart.UpdateItemQuantity;
using IdGen;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────────────────────────────────
builder.Services.AddDBContext(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);          // UserState, IdGen, Cache
builder.Services.AddScoped<BaseParameters>();
builder.Services.AddMediatRConfiguration();                              // MediatR + Validation + Transaction behaviors
builder.Services.AddFluentValidationConfiguration();                     // FluentValidation validators
builder.Services.AddMapsterConfiguration();                              // Mapster object mapping
builder.Services.AddAuthenticationConfiguration(builder.Configuration); // JWT Bearer auth
builder.Services.AddCapConfiguration(builder.Configuration);            // CAP (outbox) + RabbitMQ
builder.Services.AddControllers();

var catalogBaseUrl = builder.Configuration["CatalogService:BaseUrl"] ?? "http://catalogservice:8080/";
if (!Uri.TryCreate(catalogBaseUrl, UriKind.Absolute, out var catalogBaseAddress))
{
    throw new InvalidOperationException("CatalogService:BaseUrl must be an absolute URL.");
}

builder.Services.AddHttpClient<IProductCatalogClient, CatalogProductClient>(client =>
{
    client.BaseAddress = new Uri(catalogBaseAddress, ".");
    client.Timeout = TimeSpan.FromSeconds(5);
});

var machineId = builder.Configuration.GetValue<int?>("IdGen:MachineId") ?? 1;
if (machineId is < 0 or > 1023)
{
    throw new InvalidOperationException("IdGen:MachineId must be between 0 and 1023.");
}

builder.Services.AddSingleton<IIdGenerator<long>>(_ => new IdGenerator(machineId));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Cart API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter a valid JWT bearer token."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
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

app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapAddItemEndpoint();
app.MapUpdateItemQuantityEndpoint();
app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "Healthy", service = "Cart Service", timestamp = DateTime.UtcNow }));

app.Run();
