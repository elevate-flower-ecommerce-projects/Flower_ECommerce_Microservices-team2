using AuthService.Entities;
using AuthService.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    // ─── DbSets ────────────────────────────────────────────────────────────────
    public DbSet<Lookup> Lookups => Set<Lookup>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<DriverUser> DriverUsers => Set<DriverUser>();
    public DbSet<UserDocument> UserDocuments => Set<UserDocument>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<OtpVerificationCode> OtpVerificationCodes => Set<OtpVerificationCode>();
    public DbSet<AdminLog> AdminLogs => Set<AdminLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ─── Global Soft-Delete Query Filters ──────────────────────────────────
        modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
        modelBuilder.Entity<Role>().HasQueryFilter(r => !r.IsDeleted);
        modelBuilder.Entity<DriverUser>().HasQueryFilter(d => !d.IsDeleted);
        modelBuilder.Entity<UserDocument>().HasQueryFilter(d => !d.IsDeleted);
        modelBuilder.Entity<RefreshToken>().HasQueryFilter(t => !t.IsDeleted);
        modelBuilder.Entity<OtpVerificationCode>().HasQueryFilter(o => !o.IsDeleted);
        modelBuilder.Entity<AdminLog>().HasQueryFilter(a => !a.IsDeleted);

        // ─── User ───────────────────────────────────────────────────────────────
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Id)
                  .ValueGeneratedNever(); // Snowflake IDs are assigned before insert

            entity.Property(u => u.FullName)
                  .IsRequired()
                  .HasMaxLength(150);

            entity.Property(u => u.Email)
                  .IsRequired()
                  .HasMaxLength(256);

            entity.HasIndex(u => u.Email)
                  .IsUnique();

            entity.Property(u => u.PhoneNumber)
                  .IsRequired()
                  .HasMaxLength(20);

            entity.HasIndex(u => u.PhoneNumber)
                  .IsUnique();

            entity.Property(u => u.Password)
                  .IsRequired()
                  .HasMaxLength(512);

            entity.Property(u => u.Gender)
                  .HasConversion<string>();
        });

        // ─── Role ───────────────────────────────────────────────────────────────
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(r => r.Id);

            entity.Property(r => r.Id)
                  .ValueGeneratedNever();

            entity.Property(r => r.Name)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.HasIndex(r => r.Name)
                  .IsUnique();
        });

        // ─── UserRole (M:M join table) ─────────────────────────────────────────
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(ur => new { ur.UserId, ur.RoleId });

            entity.HasOne(ur => ur.User)
                  .WithMany(u => u.UserRoles)
                  .HasForeignKey(ur => ur.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ur => ur.Role)
                  .WithMany(r => r.UserRoles)
                  .HasForeignKey(ur => ur.RoleId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ─── DriverUser (1:1 with User) ────────────────────────────────────────
        modelBuilder.Entity<DriverUser>(entity =>
        {
            entity.HasKey(d => d.Id);

            entity.Property(d => d.Id)
                  .ValueGeneratedNever();

            entity.HasOne(d => d.User)
                  .WithOne(u => u.DriverProfile)
                  .HasForeignKey<DriverUser>(d => d.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Property(d => d.NationalId)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.HasIndex(d => d.NationalId)
                  .IsUnique();

            entity.Property(d => d.VehicleType)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(d => d.VehiclePlate)
                  .IsRequired()
                  .HasMaxLength(20);

            entity.Property(d => d.Status)
                  .HasConversion<string>();
        });

        // ─── UserDocument (M:1 with User) ─────────────────────────────────────
        modelBuilder.Entity<UserDocument>(entity =>
        {
            entity.HasKey(d => d.Id);

            entity.Property(d => d.Id)
                  .ValueGeneratedNever();

            entity.HasOne(d => d.User)
                  .WithMany(u => u.Documents)
                  .HasForeignKey(d => d.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Property(d => d.DocumentUrl)
                  .IsRequired()
                  .HasMaxLength(1024);

            entity.Property(d => d.DocumentType)
                  .HasConversion<string>();
        });

        // ─── RefreshToken (M:1 with User) ─────────────────────────────────────
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(t => t.Id);

            entity.Property(t => t.Id)
                  .ValueGeneratedNever();

            entity.HasOne(t => t.User)
                  .WithMany(u => u.RefreshTokens)
                  .HasForeignKey(t => t.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Property(t => t.Token)
                  .IsRequired()
                  .HasMaxLength(512);

            entity.HasIndex(t => t.Token)
                  .IsUnique();
        });

        // ─── OtpVerificationCode (M:1 with User) ──────────────────────────────
        modelBuilder.Entity<OtpVerificationCode>(entity =>
        {
            entity.HasKey(o => o.Id);

            entity.Property(o => o.Id)
                  .ValueGeneratedNever();

            entity.HasOne(o => o.User)
                  .WithMany(u => u.OtpCodes)
                  .HasForeignKey(o => o.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Property(o => o.GeneratedCode)
                  .IsRequired()
                  .HasMaxLength(10);
        });

        // ─── AdminLog (M:1 with User) ──────────────────────────────────────────
        modelBuilder.Entity<AdminLog>(entity =>
        {
            entity.HasKey(a => a.Id);

            entity.Property(a => a.Id)
                  .ValueGeneratedNever();

            entity.HasOne(a => a.User)
                  .WithMany()
                  .HasForeignKey(a => a.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.Property(a => a.IpAddress)
                  .IsRequired()
                  .HasMaxLength(45); // supports IPv6
        });

        // ─── Lookup (Generic enum lookup table) ────────────────────────────────
        modelBuilder.Entity<Lookup>(entity =>
        {
            entity.HasKey(l => l.Id);

            entity.Property(l => l.Id)
                  .ValueGeneratedNever(); // IDs are deterministic: (LookupType * 100) + Code

            entity.Property(l => l.LookupType)
                  .IsRequired()
                  .HasConversion<string>()
                  .HasMaxLength(50);

            entity.Property(l => l.Name)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(l => l.Description)
                  .HasMaxLength(500);

            // Ensure no duplicate Code within the same LookupType
            entity.HasIndex(l => new { l.LookupType, l.Code })
                  .IsUnique();

            // ── Seed Data ───────────────────────────────────────────────────────
            entity.HasData(

                // ── Gender (LookupType = 1 → base Id 100) ──────────────────────
                new Lookup
                {
                    Id           = 100,
                    LookupType   = LookupType.Gender,
                    Code         = (int)Gender.Male,
                    Name         = "Male",
                    Description  = "Identifies as male.",
                    DisplayOrder = 1,
                    IsActive     = true
                },
                new Lookup
                {
                    Id           = 101,
                    LookupType   = LookupType.Gender,
                    Code         = (int)Gender.Female,
                    Name         = "Female",
                    Description  = "Identifies as female.",
                    DisplayOrder = 2,
                    IsActive     = true
                },

                // ── DocumentType (LookupType = 2 → base Id 200) ────────────────
                new Lookup
                {
                    Id           = 200,
                    LookupType   = LookupType.DocumentType,
                    Code         = (int)DocumentType.NationalId,
                    Name         = "National ID",
                    Description  = "Government-issued national identity card.",
                    DisplayOrder = 1,
                    IsActive     = true
                },
                new Lookup
                {
                    Id           = 201,
                    LookupType   = LookupType.DocumentType,
                    Code         = (int)DocumentType.Passport,
                    Name         = "Passport",
                    Description  = "International travel passport.",
                    DisplayOrder = 2,
                    IsActive     = true
                },
                new Lookup
                {
                    Id           = 202,
                    LookupType   = LookupType.DocumentType,
                    Code         = (int)DocumentType.DrivingLicense,
                    Name         = "Driving License",
                    Description  = "Official motor vehicle driving license.",
                    DisplayOrder = 3,
                    IsActive     = true
                },
                new Lookup
                {
                    Id           = 203,
                    LookupType   = LookupType.DocumentType,
                    Code         = (int)DocumentType.VehicleRegistration,
                    Name         = "Vehicle Registration",
                    Description  = "Official vehicle registration document.",
                    DisplayOrder = 4,
                    IsActive     = true
                },
                new Lookup
                {
                    Id           = 204,
                    LookupType   = LookupType.DocumentType,
                    Code         = (int)DocumentType.Other,
                    Name         = "Other",
                    Description  = "Any other supporting document.",
                    DisplayOrder = 5,
                    IsActive     = true
                },

                // ── DriverStatus (LookupType = 3 → base Id 300) ────────────────
                new Lookup
                {
                    Id           = 300,
                    LookupType   = LookupType.DriverStatus,
                    Code         = (int)DriverStatus.Pending,
                    Name         = "Pending",
                    Description  = "Application submitted and awaiting review.",
                    DisplayOrder = 1,
                    IsActive     = true
                },
                new Lookup
                {
                    Id           = 301,
                    LookupType   = LookupType.DriverStatus,
                    Code         = (int)DriverStatus.Approved,
                    Name         = "Approved",
                    Description  = "Driver application approved and active.",
                    DisplayOrder = 2,
                    IsActive     = true
                },
                new Lookup
                {
                    Id           = 302,
                    LookupType   = LookupType.DriverStatus,
                    Code         = (int)DriverStatus.Rejected,
                    Name         = "Rejected",
                    Description  = "Driver application was rejected.",
                    DisplayOrder = 3,
                    IsActive     = true
                },
                new Lookup
                {
                    Id           = 303,
                    LookupType   = LookupType.DriverStatus,
                    Code         = (int)DriverStatus.Suspended,
                    Name         = "Suspended",
                    Description  = "Driver account has been suspended.",
                    DisplayOrder = 4,
                    IsActive     = true
                }
            );
        });
    }
}
