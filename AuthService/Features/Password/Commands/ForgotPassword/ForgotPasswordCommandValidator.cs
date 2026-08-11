using FluentValidation;

namespace AuthService.Features.Password.Commands.ForgotPassword
{
    /// <summary>
    /// Rules: Email is required, must be valid email format.
    /// </summary>
    public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
    {
        public ForgotPasswordCommandValidator()
        {
            RuleFor(x => x.Email)
             .NotEmpty().WithMessage("Email is required.")
             .EmailAddress().WithMessage("A valid email address is required.");
        }
    }
}
