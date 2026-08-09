using AuthService.Common.Enums;
using AuthService.Common.Interface;
using AuthService.Common.ResultPattern;
using MediatR;

namespace AuthService.Features.Users.UpdateProfile;

public sealed record UpdateProfileCommand(
    string? FullName,
    string? Email,
    string? Phone,
    Gender? Gender,
    UpdateProfileDocument? Document) : ICommand<RequestResult<UserProfileResponse>>;

public sealed record UpdateProfileDocument(
    string DocumentUrl,
    string DocumentType,
    long DocumentSize);

