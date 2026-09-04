using Catalog_Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog_Service.Data.ModelConfiguration
{
    public class ProductStoreInventoryConfiguration : IEntityTypeConfiguration<ProductStoreInventory>
    {
        public void Configure(EntityTypeBuilder<ProductStoreInventory> builder)
        {
            builder.HasKey(psi => psi.Id);

            builder.Property(psi => psi.Id)
                   .ValueGeneratedNever(); // Snowflake IDs are assigned before insert

            builder.Property(psi => psi.StockQuantity)
                   .HasDefaultValue(0);

            builder.Property(psi => psi.PriceOverride)
                   .HasPrecision(18, 2);

            builder.Property(psi => psi.IsActive)
                   .HasDefaultValue(true);

            // One stock row per (product, store) — the lookup the details endpoint drives off.
            builder.HasIndex(psi => new { psi.ProductId, psi.StoreId })
                   .IsUnique();

            builder.HasOne(psi => psi.Product)
                   .WithMany(p => p.StoreInventories)
                   .HasForeignKey(psi => psi.ProductId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(psi => psi.Store)
                   .WithMany(s => s.ProductInventories)
                   .HasForeignKey(psi => psi.StoreId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
