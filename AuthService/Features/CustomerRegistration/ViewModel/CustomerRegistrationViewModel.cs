using AuthService.Common.Enums;
using FluentValidation;
using System.Numerics;

namespace AuthService.Features.CustomerRegistration.ViewModel
{
    public class CustomerRegistrationViewModel
    {

        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public Gender Gender { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
