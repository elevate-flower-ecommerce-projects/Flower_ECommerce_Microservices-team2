using AuthService.Common.Interface;
using AuthService.Common.ResultPattern;

namespace AuthService.Features.Password.Commands.ResetPassword
{
    /// <summary>
    /// Contains Email, OtpCode, NewPassword.
    /// </summary>
    public sealed record ResetPasswordCommand(string Email, string OtpCode, string NewPassword) : ICommand<RequestResult<bool>>;
    
}
