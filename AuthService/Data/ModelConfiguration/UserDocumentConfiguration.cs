using AuthService.Entities;
using AuthService.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Data.ModelConfiguration
{
    public class UserDocumentConfiguration : IEntityTypeConfiguration<UserDocument>
    {
        public void Configure(EntityTypeBuilder<UserDocument> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Id)
                   .ValueGeneratedNever();

            builder.HasOne(d => d.User)
                   .WithMany(u => u.Documents)
                   .HasForeignKey(d => d.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(d => d.DocumentUrl)
                   .IsRequired()
                   .HasMaxLength(1024);

            builder.Property(d => d.DocumentType)
                   .IsRequired()
                   .HasMaxLength(100);
        }
    }
}
