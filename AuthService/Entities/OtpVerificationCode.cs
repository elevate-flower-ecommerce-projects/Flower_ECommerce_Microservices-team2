namespace AuthService.Entities;

/// <summary>
/// Stores OTP verification codes for email/phone confirmation and password reset.
/// </summary>
public class OtpVerificationCode : BaseEntity
{
    public string GeneratedCode { get; set; } = string.Empty;

    public long UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTime ExpireDate { get; set; }
}
