namespace AuthService.Entities.Enums;

/// <summary>
/// Identifies which category/group a lookup value belongs to.
/// Each value maps to a set of <see cref="Lookup"/> rows in the database.
/// </summary>
public enum LookupType
{
    Gender = 1,
    DocumentType = 2,
    DriverStatus = 3
}
