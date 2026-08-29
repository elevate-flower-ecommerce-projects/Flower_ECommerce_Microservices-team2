using System.ComponentModel.DataAnnotations;

namespace AuthService.Features.Password.Dto
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    public class VerifyOtpRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(6, MinimumLength = 6)]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP must be a 6-digit number.")]
        public string Otp { get; set; } = string.Empty;
    }

    public class VerifyOtpResponse
    {
        public string ResetToken { get; set; } = string.Empty;
        public DateTime ExpiresAtUtc { get; set; }
    }

    public class ResetPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string ResetCode { get; set; } = string.Empty;

        [Required]
        public string NewPassword { get; set; } = string.Empty;
    }
}
