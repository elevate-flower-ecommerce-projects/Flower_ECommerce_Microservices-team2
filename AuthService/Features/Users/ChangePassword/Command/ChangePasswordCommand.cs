using AuthService.Common.Interface;
using AuthService.Common.ResultPattern;

public sealed record ChangePasswordCommand(
    string CurrentPassword,
    string NewPassword,
    string ConfirmNewPassword) : ICommand<RequestResult<bool>>;