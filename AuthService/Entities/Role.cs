namespace AuthService.Entities;

using AuthService.Common.Enums;

public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public PersonTypeEnum PersonType { get; set; }

    // Navigation properties
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
