using AuthService.Common.Enums;

namespace AuthService.Features.Users.UpdateProfile;

public sealed class UserProfileResponse
{
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public string Status { get; set; } = string.Empty;
}

public sealed record UserDocumentResponse(
    long Id,
    string DocumentUrl,
    string DocumentType,
    long DocumentSize);