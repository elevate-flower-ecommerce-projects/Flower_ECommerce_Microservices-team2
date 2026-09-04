using AuthService.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Data.ModelConfiguration
{
    public class OtpVerificationCodeConfiguration : IEntityTypeConfiguration<OtpVerificationCode>
    {
        public void Configure(EntityTypeBuilder<OtpVerificationCode> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                   .ValueGeneratedNever();

            builder.HasOne(o => o.User)
                   .WithMany(u => u.OtpCodes)
                   .HasForeignKey(o => o.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(o => o.GeneratedCode)
                   .IsRequired()
                   .HasMaxLength(128);
        }
    }
}
