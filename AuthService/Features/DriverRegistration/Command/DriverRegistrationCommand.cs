using AuthService.Common.Enums;
using AuthService.Common.Interface;
using AuthService.Common.ResultPattern;
using FluentValidation;

namespace AuthService.Features.DriverRegistration.Command
{
    public record DriverRegistrationCommand(
        string FullName,
        string Email,
        string PhoneNumber,
        Gender Gender,
        string NationalId,
        string VehicleType,
        string VehiclePlate,
        string Password,
        string ConfirmPassword) : ICommand<RequestResult<bool>>;

    public class DriverRegistrationCommandValidator : AbstractValidator<DriverRegistrationCommand>
    {
        public DriverRegistrationCommandValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().MaximumLength(100).WithMessage("Full name is required.");
            RuleFor(x => x.Gender).IsInEnum().WithMessage("Gender must be a valid value.");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("A valid email address is required.");
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches("^(010|011|012|015)\\d{8}$")
                .WithMessage("Phone number must start with 010, 011, 012 or 015 and have a total length of 11 digits.");
            RuleFor(x => x.NationalId)
                .NotEmpty().WithMessage("National ID is required.")
                .MaximumLength(50).WithMessage("National ID cannot exceed 50 characters.");
            RuleFor(x => x.VehicleType)
                .NotEmpty().WithMessage("Vehicle type is required.")
                .MaximumLength(100).WithMessage("Vehicle type cannot exceed 100 characters.");
            RuleFor(x => x.VehiclePlate)
                .NotEmpty().WithMessage("Vehicle plate is required.")
                .MaximumLength(20).WithMessage("Vehicle plate cannot exceed 20 characters.");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one digit.")
                .Matches(@"[!@#$%^&*()_+\-=\[\]{};':""\|,.<>\/?]").WithMessage("Password must contain at least one special character.");
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Passwords do not match.");
        }
    }
}
