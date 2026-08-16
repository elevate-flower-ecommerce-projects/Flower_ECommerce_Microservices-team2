using AuthService.Common.Enums;

namespace AuthService.Entities;

/// <summary>
/// Status entity representing status lookup values in the database.
/// </summary>
public class Status : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public StatusEnum StatusType { get; set; }

    // Navigation properties
    public ICollection<DriverUser> DriverUsers { get; set; } = new List<DriverUser>();
}
