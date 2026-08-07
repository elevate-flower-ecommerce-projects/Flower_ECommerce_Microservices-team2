using AuthService.Common.Enums;
using AuthService.Common.Interface;
using AuthService.Common.ResultPattern;
using FluentValidation;

namespace AuthService.Features.CustomerRegistration.Command
{
    public record CustomerRegistrationCommand(string FullName, string Email, string PhoneNumber, Gender Gender, string Password, string ConfirmPassword) : ICommand<RequestResult<bool>>;

    public class CustomerRegistrationCommandValidator : AbstractValidator<CustomerRegistrationCommand>
    {
        public CustomerRegistrationCommandValidator()
        {

            RuleFor(x => x.FullName).NotEmpty().MaximumLength(100).WithMessage("Full name is required.");
            RuleFor(x=>x.Gender).IsInEnum().WithMessage("Gender must be a valid value.");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("A valid email address is required.");
            RuleFor(x => x.PhoneNumber).NotEmpty().Matches(@"^\+?\d{10,15}$").WithMessage("A valid phone number is required.");
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
