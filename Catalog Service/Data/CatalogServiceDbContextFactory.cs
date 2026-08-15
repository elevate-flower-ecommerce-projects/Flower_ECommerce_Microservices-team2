using Catalog_Service.Common.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Catalog_Service.Data;

/// <summary>
/// Design-time factory used by the EF Core CLI (`dotnet ef migrations` / `database update`).
/// The app itself never uses this — at runtime the context comes from DI (see ContextConfiguration).
///
/// The connection string is resolved from, in order:
///   1. ConnectionStrings__CatalogServiceDb environment variable (matches docker-compose.override.yml)
///   2. --connection "..." passed to the dotnet ef command
///   3. ConnectionStrings:CatalogServiceDb in appsettings.{ASPNETCORE_ENVIRONMENT}.json / appsettings.json
/// so the SA password never has to be committed.
/// </summary>
public class CatalogServiceDbContextFactory : IDesignTimeDbContextFactory<CatalogServiceDbContext>
{
    public CatalogServiceDbContext CreateDbContext(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddUserSecrets<CatalogServiceDbContextFactory>(optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("CatalogServiceDb");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "No 'CatalogServiceDb' connection string found. Set it before running the EF CLI, e.g.:\n" +
                "  $env:ConnectionStrings__CatalogServiceDb = \"Server=localhost,1433;Database=CatalogServiceDb;User Id=sa;Password=<SA_PASSWORD>;TrustServerCertificate=True;MultipleActiveResultSets=true;\"");
        }

        var options = new DbContextOptionsBuilder<CatalogServiceDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        // No request at design time, so an unpopulated (anonymous) UserState is correct here.
        return new CatalogServiceDbContext(options, new UserState());
    }
}
