namespace AuthService.Entities;

/// <summary>
/// Audit log capturing admin actions with user, timestamp, and IP address.
/// </summary>
public class AdminLog : BaseEntity
{
    public long UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public string IpAddress { get; set; } = string.Empty;
}
