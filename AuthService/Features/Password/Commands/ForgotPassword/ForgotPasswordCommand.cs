using AuthService.Common.Interface;
using AuthService.Common.ResultPattern;

namespace AuthService.Features.Password.Commands.ForgotPassword
{
    public record ForgotPasswordCommand(string Email) : ICommand<RequestResult<bool>>;
    
}
