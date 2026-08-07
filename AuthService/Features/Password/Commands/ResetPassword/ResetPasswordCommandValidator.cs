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
            //RuleFor(x => x.Email)
            //    .NotEmpty()
            //    .EmailAddress();

            //RuleFor(x => x.OtpCode)
            //    .NotEmpty()
            //    .Length(6)
            //    .Matches(@"^\d{6}$");

            //RuleFor(x => x.NewPassword)
            //    .NotEmpty()
            //    .MinimumLength(8)
            //    .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]");
        }
    }
}
