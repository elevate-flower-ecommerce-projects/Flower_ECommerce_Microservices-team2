using AuthService.Common.Enums;
using AuthService.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Data.ModelConfiguration
{
    public class PersonTypeConfiguration : IEntityTypeConfiguration<PersonType>
    {
        public void Configure(EntityTypeBuilder<PersonType> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                   .ValueGeneratedNever();

            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasIndex(p => p.Name)
                   .IsUnique();

            // Seed Data strongly coupled to PersonTypeEnum enum
            builder.HasData(
                new PersonType
                {
                    Id          = (long)PersonTypeEnum.Customer,
                    Name        = nameof(PersonTypeEnum.Customer),
                    Description = "Standard e-commerce customer user."
                },
                new PersonType
                {
                    Id          = (long)PersonTypeEnum.Driver,
                    Name        = nameof(PersonTypeEnum.Driver),
                    Description = "Delivery driver user profile."
                },
                new PersonType
                {
                    Id          = (long)PersonTypeEnum.Admin,
                    Name        = nameof(PersonTypeEnum.Admin),
                    Description = "System administrator user."
                }
            );
        }
    }
}
