using AuthService.Common.Interface;
using AuthService.Common.ResultPattern;
using AuthService.Entities;

namespace AuthService.Features.UserManagement.LoginFeature.Command
{
    public record GenerateRefreshTokenCommand(User User) : ICommand<RequestResult<string>>;
}
