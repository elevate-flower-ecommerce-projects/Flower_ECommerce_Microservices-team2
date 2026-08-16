using AuthService.Common.Enums;
using AuthService.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Data.ModelConfiguration
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
                   .ValueGeneratedNever();

            builder.Property(r => r.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasIndex(r => r.Name)
                   .IsUnique();

            builder.Property(r => r.PersonType)
                   .HasConversion<string>()
                   .IsRequired();

            builder.HasQueryFilter(r => !r.IsDeleted);

            // Seed Data strongly coupled to PersonTypeEnum
            builder.HasData(
                new Role
                {
                    Id         = (long)PersonTypeEnum.Customer,
                    Name       = nameof(PersonTypeEnum.Customer),
                    PersonType = PersonTypeEnum.Customer
                },
                new Role
                {
                    Id         = (long)PersonTypeEnum.Driver,
                    Name       = nameof(PersonTypeEnum.Driver),
                    PersonType = PersonTypeEnum.Driver
                },
                new Role
                {
                    Id         = (long)PersonTypeEnum.Admin,
                    Name       = nameof(PersonTypeEnum.Admin),
                    PersonType = PersonTypeEnum.Admin
                }
            );
        }
    }
}
