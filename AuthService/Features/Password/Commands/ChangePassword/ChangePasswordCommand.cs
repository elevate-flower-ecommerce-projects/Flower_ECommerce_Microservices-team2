using AuthService.Common.Interface;
using AuthService.Common.ResultPattern;

namespace AuthService.Features.Password.Commands.ChangePassword
{
    public record ChangePasswordCommand(string CurrentPassword, string NewPassword) : ICommand<RequestResult<bool>>;
    
}
