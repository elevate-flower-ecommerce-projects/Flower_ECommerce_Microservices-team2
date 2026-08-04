using AuthService.Common.Enums;

namespace AuthService.Entities;

/// <summary>
/// Represents a user classification or person type in the system (e.g. Customer, Driver, Admin).
/// </summary>
public class PersonType : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public PersonTypeEnum PersonTypeValue { get; set; }

    // Navigation properties
    public ICollection<User> Users { get; set; } = new List<User>();
}
