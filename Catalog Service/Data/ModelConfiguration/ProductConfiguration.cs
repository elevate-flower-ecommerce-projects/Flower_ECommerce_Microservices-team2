using Catalog_Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog_Service.Data.ModelConfiguration
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                   .ValueGeneratedNever(); // Snowflake IDs are assigned before insert

            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(p => p.Price)
                   .IsRequired()
                   .HasPrecision(18, 2);

            builder.Property(p => p.DiscountPercentage)
                   .HasPrecision(5, 2);

            builder.Property(p => p.ProductStatus)
                   .HasConversion<string>()
                   .HasMaxLength(50);

            builder.Property(p => p.Quantity)
                   .HasDefaultValue(0);

            builder.Property(p => p.Description)
                   .HasMaxLength(2000);

            builder.Property(p => p.IsArchived)
                   .HasDefaultValue(false);

            builder.HasIndex(p => p.Name);

            builder.HasOne(p => p.Category)
                   .WithMany(c => c.Products)
                   .HasForeignKey(p => p.CategoryId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
