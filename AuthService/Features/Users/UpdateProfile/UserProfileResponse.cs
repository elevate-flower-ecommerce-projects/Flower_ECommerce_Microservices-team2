using AuthService.Common.Enums;

namespace AuthService.Features.Users.UpdateProfile;

public sealed record UserProfileResponse(
    long Id,
    string FullName,
    string Email,
    string Phone,
    Gender Gender,
    UserDocumentResponse? Document);

public sealed record UserDocumentResponse(
    long Id,
    string DocumentUrl,
    string DocumentType,
    long DocumentSize);