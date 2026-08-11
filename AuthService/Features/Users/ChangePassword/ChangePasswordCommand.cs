using AuthService.Common.Interface;
using AuthService.Common.ResultPattern;
using FluentValidation;

namespace AuthService.Features.Users.ChangePassword;

public sealed record ChangePasswordCommand(
    string CurrentPassword,
    string NewPassword,
    string ConfirmNewPassword) : ICommand<RequestResult<bool>>;

public sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(command => command.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required.");
    }
}