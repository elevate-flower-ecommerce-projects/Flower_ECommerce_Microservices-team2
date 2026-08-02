using AuthService.Entities.Enums;

namespace AuthService.Entities;

/// <summary>
/// Represents a driver's extended profile linked 1:1 to a User.
/// </summary>
public class DriverUser : BaseEntity
{
    /// <summary>Foreign key to the base User record.</summary>
    public long UserId { get; set; }
    public User User { get; set; } = null!;

    public string NationalId { get; set; } = string.Empty;

    public string VehicleType { get; set; } = string.Empty;

    public string VehiclePlate { get; set; } = string.Empty;

    public string? RejectionReason { get; set; }

    public DriverStatus Status { get; set; } = DriverStatus.Pending;
}
