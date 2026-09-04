using Catalog_Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog_Service.Data.ModelConfiguration
{
    public class StoreConfiguration : IEntityTypeConfiguration<Store>
    {
        public void Configure(EntityTypeBuilder<Store> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id)
                   .ValueGeneratedNever(); // Ids mirror the Address & Store Coverage service

            builder.Property(s => s.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(s => s.CoverageArea)
                   .HasMaxLength(200);

            builder.Property(s => s.IsActive)
                   .HasDefaultValue(true);

            builder.HasIndex(s => s.CoverageArea);
        }
    }
}
