using AuthService.Data;
using IdGen;
using IdGen.DependencyInjection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── Database ─────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuthDb")));

// ── Snowflake ID Generator (IdGen) ────────────────────────────────────────────
// MachineId uniquely identifies this service instance in a distributed setup.
// Change MachineId (0-1023) per node/container to avoid ID collisions.
var machineId = builder.Configuration.GetValue<int>("IdGen:MachineId");
builder.Services.AddIdGen(machineId, () => new IdGeneratorOptions());

// ── MVC ───────────────────────────────────────────────────────────────────────
builder.Services.AddControllers();

// ── OpenAPI spec (.NET 10 native) ─────────────────────────────────────────────
// Generates the raw JSON spec at /openapi/v1.json
builder.Services.AddOpenApi("v1", options =>
{
    options.AddDocumentTransformer((document, context, _) =>
    {
        document.Info = new()
        {
            Title       = "Auth Service API",
            Version     = "v1",
            Description = "Handles authentication, authorization, user management, and token operations for the Flower E-Commerce platform."
        };
        return Task.CompletedTask;
    });
});

// ── Swagger UI (Swashbuckle) ──────────────────────────────────────────────────
// Swashbuckle is used ONLY for the interactive HTML UI.
// It reads the spec from /openapi/v1.json generated above.
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// ── Swagger UI middleware ──────────────────────────────────────────────────────
// Enabled in Development and Docker so the UI is reachable from containers
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
{
    app.MapOpenApi();           // → /openapi/v1.json

    app.UseSwaggerUI(ui =>
    {
        ui.SwaggerEndpoint("/openapi/v1.json", "Auth Service v1");
        ui.RoutePrefix           = "swagger";       // → http://localhost:5000/swagger
        ui.DocumentTitle         = "Auth Service API";
        ui.DisplayRequestDuration();                // shows each request's time in ms
        ui.EnableTryItOutByDefault();               // "Try it out" open by default
    });
}

// ── HTTP Pipeline ─────────────────────────────────────────────────────────────
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
