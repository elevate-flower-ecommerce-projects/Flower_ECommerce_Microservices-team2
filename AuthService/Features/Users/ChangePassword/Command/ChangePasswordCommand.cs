using AuthService.Common.Interface;
using AuthService.Common.ResultPattern;
using FluentValidation;

namespace AuthService.Features.Users.ChangePassword;

public sealed record ChangePasswordCommand(
    string CurrentPassword,
    string NewPassword) : ICommand<RequestResult<bool>>;

public sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(command => command.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required.");

        RuleFor(command => command.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .MinimumLength(6).WithMessage("New password must be at least 6 characters long.");
    }
}