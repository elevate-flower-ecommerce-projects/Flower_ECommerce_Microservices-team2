using AuthService.Features.Users.UpdateProfile;

namespace AuthService.Features.UserManagement.LoginFeature.Dto
{
    public class TokenResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
        public string? DriverStatus { get; set; }
        public UserProfileResponse User { get; set; } = new();
    }
}
