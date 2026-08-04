using AuthService.Entities;
using AuthService.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Data.ModelConfiguration
{
    public class DriverUserConfiguration : IEntityTypeConfiguration<DriverUser>
    {
        public void Configure(EntityTypeBuilder<DriverUser> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Id)
                   .ValueGeneratedNever();

            builder.HasOne(d => d.User)
                   .WithOne(u => u.DriverProfile)
                   .HasForeignKey<DriverUser>(d => d.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(d => d.NationalId)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasIndex(d => d.NationalId)
                   .IsUnique();

            builder.Property(d => d.VehicleType)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(d => d.VehiclePlate)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.HasOne(d => d.Status)
                   .WithMany(s => s.DriverUsers)
                   .HasForeignKey(d => d.StatusId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
