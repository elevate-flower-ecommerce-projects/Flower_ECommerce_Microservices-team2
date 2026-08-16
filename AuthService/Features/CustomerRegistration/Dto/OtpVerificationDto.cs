namespace AuthService.Features.CustomerRegistration.Dto
{
    public class OtpVerificationDto
    {
        public long Id { get; set; }
        public string GeneratedCode { get; set; } = string.Empty;
        public DateTime ExpireDate { get; set; }
    }
}
