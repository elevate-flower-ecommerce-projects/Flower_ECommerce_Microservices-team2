using AuthService.Common.Enums;
using AuthService.Common.Interface;
using AuthService.Common.ResultPattern;
using MediatR;

namespace AuthService.Features.Users.UpdateProfile;

public sealed record UpdateProfileCommand(
    string? FullName,
    string? Email,
    string? PhoneNumber,
    Gender? Gender,
    string? PhotoUrl) : ICommand<RequestResult<UserProfileResponse>>;

