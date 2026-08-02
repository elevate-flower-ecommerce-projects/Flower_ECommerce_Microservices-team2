using AuthService.Entities.Enums;

namespace AuthService.Entities;

/// <summary>
/// Generic lookup table that stores all enum values from the system
/// (Gender, DocumentType, DriverStatus) as database rows.
/// Useful for dropdowns, admin UIs, and API responses without hardcoding values on the client.
/// </summary>
public class Lookup
{
    /// <summary>
    /// Composite-friendly integer identity.
    /// Convention: (LookupType * 100) + Code — e.g. Gender=1, Male=0 → Id=100, Female=1 → Id=101.
    /// </summary>
    public int Id { get; set; }

    /// <summary>Which enum category this row belongs to.</summary>
    public LookupType LookupType { get; set; }

    /// <summary>
    /// Integer code that matches the underlying C# enum value.
    /// Allows safe casting: (Gender)lookup.Code
    /// </summary>
    public int Code { get; set; }

    /// <summary>Human-readable display name (e.g. "Male", "National ID").</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Optional longer description shown in tooltips or help text.</summary>
    public string? Description { get; set; }

    /// <summary>Controls sort order when rendering lists/dropdowns.</summary>
    public int DisplayOrder { get; set; }

    /// <summary>Soft-disable a lookup value without deleting it.</summary>
    public bool IsActive { get; set; } = true;
}
