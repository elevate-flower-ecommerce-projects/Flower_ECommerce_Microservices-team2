using AuthService.Common.Interface;
using AuthService.Common.ResultPattern;
using FluentValidation;

namespace AuthService.Features.CustomerRegistration.Command.Verification
{
    public record VerifyEmailCommand(long UserId, string code) : ICommand<RequestResult<bool>>;




    public class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
    {
        public VerifyEmailCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");
            RuleFor(x => x.code)
                .NotEmpty().WithMessage("Verification code is required.");
        }
      
    }
}
