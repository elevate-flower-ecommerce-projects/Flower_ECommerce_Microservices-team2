using AuthService.Common.Interface;
using AuthService.Common.ResultPattern;
using AuthService.Features.Password.Dto;

namespace AuthService.Features.Password.Commands.VerifyOtp
{
    public sealed record VerifyOtpCommand(string Email, string Otp) : ICommand<RequestResult<VerifyOtpResponse>>;
}
