using Catalog_Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog_Service.Data.ModelConfiguration
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                   .ValueGeneratedNever(); // Snowflake IDs are assigned before insert

            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.HasIndex(c => c.Name)
                   .IsUnique();

            builder.Property(c => c.ImageUrl)
                   .HasMaxLength(2048);

            builder.Property(c => c.DisplayOrder)
                   .HasDefaultValue(0);
        }
    }
}
