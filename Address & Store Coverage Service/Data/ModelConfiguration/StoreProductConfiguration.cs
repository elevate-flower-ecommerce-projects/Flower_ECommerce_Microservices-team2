using Address___Store_Coverage_Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Address___Store_Coverage_Service.Data.ModelConfiguration;

public class StoreProductConfiguration : IEntityTypeConfiguration<StoreProduct>
{
    public void Configure(EntityTypeBuilder<StoreProduct> builder)
    {
        builder.HasKey(sp => new { sp.StoreId, sp.ProductId });

        builder.HasOne(sp => sp.Store)
               .WithMany(s => s.StoreProducts)
               .HasForeignKey(sp => sp.StoreId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(sp => sp.ProductId);
    }
}
