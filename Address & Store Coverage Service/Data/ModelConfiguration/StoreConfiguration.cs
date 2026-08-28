using Address___Store_Coverage_Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Address___Store_Coverage_Service.Data.ModelConfiguration;

public class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
               .ValueGeneratedNever();

        builder.Property(s => s.Name)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(s => s.PhysicalLocation)
               .IsRequired()
               .HasMaxLength(500);

        builder.Property(s => s.CoverageArea)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(s => s.CreatedAt)
               .IsRequired();

        builder.Property(s => s.IsDeleted)
               .HasDefaultValue(false);

        builder.HasIndex(s => s.Name);
        builder.HasIndex(s => s.CoverageArea);
    }
}
