using Address___Store_Coverage_Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Address___Store_Coverage_Service.Data.ModelConfiguration;

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
               .HasMaxLength(30);

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
               .HasPrecision(9, 6);

        builder.Property(a => a.Lng)
               .HasPrecision(9, 6);

        builder.Property(a => a.IsDefault)
               .HasDefaultValue(false);

        builder.Property(a => a.CreatedAt)
               .IsRequired();

        builder.Property(a => a.IsDeleted)
               .HasDefaultValue(false);

        builder.HasIndex(a => a.UserId);
        builder.HasIndex(a => new { a.City, a.Area });
    }
}
