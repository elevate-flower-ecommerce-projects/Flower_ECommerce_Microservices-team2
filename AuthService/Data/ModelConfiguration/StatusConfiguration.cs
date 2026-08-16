using AuthService.Common.Enums;
using AuthService.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Data.ModelConfiguration
{
    public class StatusConfiguration : IEntityTypeConfiguration<Status>
    {
        public void Configure(EntityTypeBuilder<Status> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id)
                   .ValueGeneratedNever();

            builder.Property(s => s.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasIndex(s => s.Name)
                   .IsUnique();

            // Seed Data strongly coupled to DriverStatus enum
            builder.HasData(
                new Status
                {
                    Id          = (long)DriverStatus.Pending,
                    Name        = nameof(DriverStatus.Pending),
                    Description = "Application submitted and awaiting review."
                },
                new Status
                {
                    Id          = (long)DriverStatus.Approved,
                    Name        = nameof(DriverStatus.Approved),
                    Description = "Driver application approved and active."
                },
                new Status
                {
                    Id          = (long)DriverStatus.Rejected,
                    Name        = nameof(DriverStatus.Rejected),
                    Description = "Driver application was rejected."
                },
                new Status
                {
                    Id          = (long)DriverStatus.Suspended,
                    Name        = nameof(DriverStatus.Suspended),
                    Description = "Driver account has been suspended."
                }
            );
        }
    }
}
