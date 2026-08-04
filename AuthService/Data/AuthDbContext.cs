using AuthService.Common.Services;
using AuthService.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AuthService.Data;

public class AuthDbContext : DbContext
{
    private readonly CurrentUserService _currentUserService;

    public AuthDbContext(DbContextOptions<AuthDbContext> options, CurrentUserService currentUserService)
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    // ─── Override OnConfiguring to apply AsNoTracking globally ──────────────────
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        optionsBuilder.ConfigureWarnings(w => 
            w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)
        );
        base.OnConfiguring(optionsBuilder);
    }

    // ─── DbSets ────────────────────────────────────────────────────────────────
    public DbSet<Status>               Statuses              => Set<Status>();
    public DbSet<PersonType>           PersonTypes           => Set<PersonType>();
    public DbSet<User>                 Users                 => Set<User>();
    public DbSet<Role>                 Roles                 => Set<Role>();
    public DbSet<UserRole>             UserRoles             => Set<UserRole>();
    public DbSet<DriverUser>           DriverUsers           => Set<DriverUser>();
    public DbSet<UserDocument>         UserDocuments         => Set<UserDocument>();
    public DbSet<RefreshToken>         RefreshTokens         => Set<RefreshToken>();
    public DbSet<OtpVerificationCode>  OtpVerificationCodes  => Set<OtpVerificationCode>();
    public DbSet<AdminLog>             AdminLogs             => Set<AdminLog>();

    // ─── Audit: auto-stamp CreatedBy / UpdatedBy on every save ────────────────
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = _currentUserService.UserId;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = _currentUserService.UserId;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    // ─── Model Configuration ───────────────────────────────────────────────────
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Automatically discovers and applies all IEntityTypeConfiguration<T>
        // classes in this assembly (Data/ModelConfiguration/*.cs)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);

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
