using AuthService.Common.Enums;

namespace AuthService.Features.UserManagement.LoginFeature.Dto
{
    // DTO containing password for internal authentication use only.
    public class UserAuthDto
    {
        public long Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public string Password { get; set; } = string.Empty;
    }
}
