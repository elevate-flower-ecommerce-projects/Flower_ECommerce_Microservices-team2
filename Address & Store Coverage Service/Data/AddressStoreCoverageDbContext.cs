using Address___Store_Coverage_Service.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Address___Store_Coverage_Service.Data;

public class AddressStoreCoverageDbContext : DbContext
{
    public AddressStoreCoverageDbContext(DbContextOptions<AddressStoreCoverageDbContext> options)
        : base(options)
    {
    }

    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Store> Stores => Set<Store>();
    public DbSet<StoreProduct> StoreProducts => Set<StoreProduct>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution);
        optionsBuilder.ConfigureWarnings(w =>
            w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AddressStoreCoverageDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                var filter = Expression.Lambda(Expression.Not(property), parameter);

                entityType.SetQueryFilter(filter);
            }
        }

        base.OnModelCreating(modelBuilder);
    }
}
