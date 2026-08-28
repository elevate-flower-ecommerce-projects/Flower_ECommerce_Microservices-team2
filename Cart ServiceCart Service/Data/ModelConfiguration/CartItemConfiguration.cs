using Cart_ServiceCart_Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cart_ServiceCart_Service.Data.ModelConfiguration;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.HasKey(ci => ci.Id);

        builder.Property(ci => ci.Id)
               .ValueGeneratedNever();

        builder.Property(ci => ci.ProductId)
               .IsRequired();

        builder.Property(ci => ci.TotalPrice)
               .IsRequired()
               .HasPrecision(18, 2);

        builder.Property(ci => ci.Quantity)
               .IsRequired()
               .HasDefaultValue(1);

        builder.Property(ci => ci.CreatedAt)
               .IsRequired();

        builder.Property(ci => ci.IsDeleted)
               .HasDefaultValue(false);

        builder.HasIndex(ci => ci.ProductId);
        builder.HasIndex(ci => new { ci.CartId, ci.ProductId })
               .IsUnique();
    }
}
