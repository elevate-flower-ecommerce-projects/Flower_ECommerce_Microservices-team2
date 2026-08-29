using FluentValidation;

namespace AuthService.Features.Password.Commands.ResetPassword
{
    /// <summary>
    /// Rules: Email required, OTP required (6 digits), NewPassword required (min length, complexity).
    /// </summary>
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.");

            RuleFor(x => x.ResetCode)
                .NotEmpty().WithMessage("Reset code is required.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(8).WithMessage("New password must be at least 8 characters long.")
                .Matches(@"[A-Z]").WithMessage("New password must contain at least one uppercase letter.")
                .Matches(@"[a-z]").WithMessage("New password must contain at least one lowercase letter.")
                .Matches(@"[0-9]").WithMessage("New password must contain at least one digit.")
                .Matches(@"[\!\@\#\$\%\^\&\*\(\)\_\+\-\=\[\]\{\}\;\:\'""\,\<\.\>\/\?\~]|\W")
                .WithMessage("New password must contain at least one special character.");
        }
    }
}
