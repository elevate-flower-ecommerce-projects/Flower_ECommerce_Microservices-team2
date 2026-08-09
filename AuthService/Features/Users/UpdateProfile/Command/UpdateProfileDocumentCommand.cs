using AuthService.Common.Enums;
using AuthService.Common.Interface;
using AuthService.Common.ResultPattern;
using MediatR;

namespace AuthService.Features.Users.UpdateProfile;



public sealed record UpdateProfileDocumentCommand(
    UpdateProfileDocument Document) : ICommand<RequestResult<UserDocumentResponse>>;