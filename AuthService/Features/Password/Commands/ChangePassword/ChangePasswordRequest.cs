namespace AuthService.Features.Password.Commands.ChangePassword
{
    public sealed class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
