using AuthService.Entities.Enums;

namespace AuthService.Entities;

/// <summary>
/// Represents a registered user in the system.
/// </summary>
public class User : BaseEntity
{
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public Gender Gender { get; set; }

    public string Password { get; set; } = string.Empty;

    public bool IsEmailConfirmed { get; set; } = false;

    public bool IsBlocked { get; set; } = false;

    public DateTime? BlockedAt { get; set; }

    // Navigation properties
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<OtpVerificationCode> OtpCodes { get; set; } = new List<OtpVerificationCode>();
    public ICollection<UserDocument> Documents { get; set; } = new List<UserDocument>();
    public DriverUser? DriverProfile { get; set; }
}
