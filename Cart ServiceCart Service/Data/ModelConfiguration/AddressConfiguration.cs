using Cart_ServiceCart_Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cart_ServiceCart_Service.Data.ModelConfiguration;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
               .ValueGeneratedNever();

        builder.Property(a => a.UserId)
               .IsRequired();

        builder.Property(a => a.RecipientName)
               .IsRequired()
               .HasMaxLength(150);

        builder.Property(a => a.RecipientPhone)
               .IsRequired()
               .HasMaxLength(20);

        builder.Property(a => a.City)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(a => a.Area)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(a => a.AddressLine)
               .IsRequired()
               .HasMaxLength(500);

        builder.Property(a => a.Label)
               .HasMaxLength(50);

        builder.Property(a => a.Lat)
               .HasPrecision(10, 7);

        builder.Property(a => a.Lng)
               .HasPrecision(10, 7);

        builder.Property(a => a.IsDefault)
               .HasDefaultValue(false);

        builder.Property(a => a.StoreId);

        builder.Property(a => a.IsServiceable)
               .HasDefaultValue(true);

        builder.Property(a => a.CreatedAt)
               .IsRequired();

        builder.Property(a => a.IsDeleted)
               .HasDefaultValue(false);

        // Fast lookups by user
        builder.HasIndex(a => a.UserId);

        // Supports the default-first ordering query
        builder.HasIndex(a => new { a.UserId, a.IsDefault });

        // Lookup by resolved store
        builder.HasIndex(a => a.StoreId);
    }
}
