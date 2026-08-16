using Catalog_Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog_Service.Data.ModelConfiguration
{
    public class SectionConfiguration : IEntityTypeConfiguration<Section>
    {
        public void Configure(EntityTypeBuilder<Section> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id)
                   .ValueGeneratedNever(); // Snowflake IDs are assigned before insert

            builder.Property(s => s.Name)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.HasIndex(s => s.Name)
                   .IsUnique();

            builder.Property(s => s.Order)
                   .HasDefaultValue(0);

            builder.Property(s => s.Type)
                   .HasConversion<string>()
                   .HasMaxLength(50);

            builder.Property(s => s.IsEnabled)
                   .HasDefaultValue(true);
        }
    }
}
