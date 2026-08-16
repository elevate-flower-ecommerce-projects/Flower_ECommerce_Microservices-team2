using AuthService.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Data.ModelConfiguration
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                   .ValueGeneratedNever();

            builder.HasOne(t => t.User)
                   .WithMany(u => u.RefreshTokens)
                   .HasForeignKey(t => t.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(t => t.Token)
                   .IsRequired()
                   .HasMaxLength(512);

            builder.HasIndex(t => t.Token)
                   .IsUnique();

            builder.HasQueryFilter(t => !t.IsDeleted);
        }
    }
}
