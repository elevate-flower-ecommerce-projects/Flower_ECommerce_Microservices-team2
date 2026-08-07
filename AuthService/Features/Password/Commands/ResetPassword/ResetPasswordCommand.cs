using AuthService.Common.Interface;
using AuthService.Common.ResultPattern;

namespace AuthService.Features.Password.Commands.ResetPassword
{
    public sealed record ResetPasswordCommand(string Email, string OtpCode, string NewPassword) : ICommand<RequestResult<bool>>;
    
}
