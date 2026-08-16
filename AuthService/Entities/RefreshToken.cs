namespace AuthService.Entities;

/// <summary>
/// Stores JWT refresh tokens for persistent authentication sessions.
/// </summary>
public class RefreshToken : BaseEntity
{
    public string Token { get; set; } = string.Empty;

    public long UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTime ExpireDate { get; set; }

    /// <summary>Whether the user chose "Remember Me" on login.</summary>
    public bool IsRemember { get; set; } = false;
}
