using Catalog_Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog_Service.Data.ModelConfiguration
{
    public class ProductIncludeConfiguration : IEntityTypeConfiguration<ProductInclude>
    {
        public void Configure(EntityTypeBuilder<ProductInclude> builder)
        {
            builder.HasKey(pi => pi.Id);

            builder.Property(pi => pi.Id)
                   .ValueGeneratedNever(); // Snowflake IDs are assigned before insert

            builder.Property(pi => pi.Item)
                   .IsRequired()
                   .HasMaxLength(300);

            builder.Property(pi => pi.Quantity)
                   .HasDefaultValue(1);

            builder.Property(pi => pi.DisplayOrder)
                   .HasDefaultValue(0);

            builder.HasIndex(pi => new { pi.ProductId, pi.DisplayOrder });

            builder.HasOne(pi => pi.Product)
                   .WithMany(p => p.Includes)
                   .HasForeignKey(pi => pi.ProductId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
