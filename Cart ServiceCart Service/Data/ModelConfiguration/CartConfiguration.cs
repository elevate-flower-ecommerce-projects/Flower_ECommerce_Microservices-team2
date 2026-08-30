using Cart_ServiceCart_Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cart_ServiceCart_Service.Data.ModelConfiguration;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
               .ValueGeneratedNever();

        builder.Property(c => c.UserId)
               .IsRequired();

        builder.Property(c => c.TotalPrice)
               .IsRequired()
               .HasPrecision(18, 2);

        builder.Property(c => c.IsCheckOut)
               .HasDefaultValue(false);

        builder.Property(c => c.CreatedAt)
               .IsRequired();

        builder.Property(c => c.IsDeleted)
               .HasDefaultValue(false);

        builder.HasIndex(c => c.UserId);

        builder.HasMany(c => c.CartItems)
               .WithOne(ci => ci.Cart)
               .HasForeignKey(ci => ci.CartId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
