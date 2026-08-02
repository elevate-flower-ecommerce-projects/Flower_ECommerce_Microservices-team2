using AuthService.Entities.Enums;

namespace AuthService.Entities;

/// <summary>
/// Represents an uploaded verification document belonging to a User.
/// </summary>
public class UserDocument : BaseEntity
{
    public long UserId { get; set; }
    public User User { get; set; } = null!;

    /// <summary>File size in bytes.</summary>
    public long DocumentSize { get; set; }

    public DocumentType DocumentType { get; set; }

    /// <summary>URL or path to the stored document file.</summary>
    public string DocumentUrl { get; set; } = string.Empty;
}
