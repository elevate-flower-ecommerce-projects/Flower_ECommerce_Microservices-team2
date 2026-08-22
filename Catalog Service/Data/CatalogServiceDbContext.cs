using Catalog_Service.Common.Services;
using Catalog_Service.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Catalog_Service.Data;

public class CatalogServiceDbContext : DbContext
{
    private readonly IUserState _userState;

    public CatalogServiceDbContext(DbContextOptions<CatalogServiceDbContext> options, IUserState userState)
        : base(options)
    {
        _userState = userState;
    }

    // ─── Entity Sets ───────────────────────────────────────────────────────────
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<Occasion> Occasions => Set<Occasion>();
    public DbSet<ProductOccasion> ProductOccasions => Set<ProductOccasion>();
    public DbSet<Section> Sections => Set<Section>();

    // ─── Override OnConfiguring to apply AsNoTracking globally ──────────────────
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution);
        optionsBuilder.ConfigureWarnings(w => 
            w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)
        );
        base.OnConfiguring(optionsBuilder);
    }

    // ─── Model Configuration ───────────────────────────────────────────────────
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Automatically discovers and applies all IEntityTypeConfiguration<T>
        // classes in this assembly (Data/ModelConfiguration/*.cs)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogServiceDbContext).Assembly);

        // Apply global query filter for soft deletes on all BaseEntity derived entities
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter =Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                var filter =   Expression.Lambda(Expression.Not(property), parameter);
                                        

                entityType.SetQueryFilter(filter);
            }
        }

        base.OnModelCreating(modelBuilder);
    }
}
