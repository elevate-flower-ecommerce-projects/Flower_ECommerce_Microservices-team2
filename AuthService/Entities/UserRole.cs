namespace AuthService.Entities;

/// <summary>
/// Join table for the many-to-many relationship between User and Role.
/// </summary>
public class UserRole
{
    public long UserId { get; set; }
    public User User { get; set; } = null!;

    public long RoleId { get; set; }
    public Role Role { get; set; } = null!;
}
