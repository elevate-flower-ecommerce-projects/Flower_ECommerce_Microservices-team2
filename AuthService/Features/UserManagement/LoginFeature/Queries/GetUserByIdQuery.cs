using AuthService.Common.ResultPattern;
using AuthService.Features.UserManagement.LoginFeature.Dto;
using MediatR;

namespace AuthService.Features.UserManagement.LoginFeature.Queries
{
    public record GetUserByIdQuery(long UserId) : IRequest<RequestResult<UserDto>>;
}
