using AuthService.Common.Interface;
using AuthService.Common.ResultPattern;
using AuthService.Features.UserManagement.LoginFeature.Dto;

namespace AuthService.Features.UserManagement.LoginFeature.Command
{
    public record GenerateJwtTokenCommand(UserDto User) : ICommand<RequestResult<string>>;
}
