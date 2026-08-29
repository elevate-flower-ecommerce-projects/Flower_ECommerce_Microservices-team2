using FluentValidation;

namespace AuthService.Features.Password.Commands.VerifyOtp
{
    public class VerifyOtpCommandValidator : AbstractValidator<VerifyOtpCommand>
    {
        public VerifyOtpCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Otp)
                .NotEmpty().WithMessage("OTP is required.")
                .Length(6).WithMessage("OTP must be 6 characters.")
                .Matches(@"^\d{6}$").WithMessage("OTP must be 6 digits.");
        }
    }
}
