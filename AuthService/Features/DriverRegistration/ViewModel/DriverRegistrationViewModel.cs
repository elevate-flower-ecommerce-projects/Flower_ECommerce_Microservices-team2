using AuthService.Common.Enums;

namespace AuthService.Features.DriverRegistration.ViewModel
{
    public class DriverRegistrationViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public string NationalId { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public string VehiclePlate { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
