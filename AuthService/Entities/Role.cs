namespace AuthService.Entities;

/// <summary>
/// Represents an authorization role (e.g. Admin, Driver, Customer).
/// </summary>
public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
