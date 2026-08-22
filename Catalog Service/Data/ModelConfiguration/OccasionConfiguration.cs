using Catalog_Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog_Service.Data.ModelConfiguration
{
    public class OccasionConfiguration : IEntityTypeConfiguration<Occasion>
    {
        public void Configure(EntityTypeBuilder<Occasion> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                   .ValueGeneratedNever(); // Snowflake IDs are assigned before insert

            builder.Property(o => o.Name)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.HasIndex(o => o.Name)
                   .IsUnique();

            builder.Property(o => o.ImageUrl)
                   .HasMaxLength(2048);
        }
    }
}
