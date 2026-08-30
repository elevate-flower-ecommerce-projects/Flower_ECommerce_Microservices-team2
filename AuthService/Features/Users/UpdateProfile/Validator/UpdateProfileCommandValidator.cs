using AuthService.Common.Validation;
using FluentValidation;

namespace AuthService.Features.Users.UpdateProfile;

public sealed class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(command => command)
            .Must(command => command.FullName is not null || command.Email is not null ||
                             command.PhoneNumber is not null || command.Gender is not null ||
                             command.PhotoUrl is not null)
            .WithMessage("At least one profile field must be provided.");

        When(command => command.FullName is not null, () =>
            RuleFor(command => command.FullName)
                .Must(value => !string.IsNullOrWhiteSpace(value))
                .WithMessage("fullName is required.")
                .MaximumLength(UserValidationRules.FullNameMaxLength)
                .WithMessage($"fullName must be {UserValidationRules.FullNameMaxLength} characters or fewer."));

        When(command => command.Email is not null, () =>
            RuleFor(command => command.Email)
                .EmailAddress()
                .WithMessage("email must be a valid email address.")
                .MaximumLength(UserValidationRules.EmailMaxLength)
                .WithMessage($"email must be {UserValidationRules.EmailMaxLength} characters or fewer."));

        When(command => command.PhoneNumber is not null, () =>
            RuleFor(command => command.PhoneNumber)
                .Matches(UserValidationRules.PhonePattern)
                .WithMessage("phoneNumber must be a valid international phone number.")
                .MaximumLength(UserValidationRules.PhoneMaxLength)
                .WithMessage($"phoneNumber must be {UserValidationRules.PhoneMaxLength} characters or fewer."));

        When(command => command.Gender is not null, () =>
            RuleFor(command => command.Gender)
                .IsInEnum()
                .WithMessage("gender is invalid."));

        When(command => command.PhotoUrl is not null, () =>
            RuleFor(command => command.PhotoUrl)
                .Must(value => Uri.TryCreate(value, UriKind.Absolute, out var uri) &&
                               (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                .WithMessage("photoUrl must be a served HTTP or HTTPS URL.")
                .MaximumLength(UserValidationRules.DocumentUrlMaxLength)
                .WithMessage($"photoUrl must be {UserValidationRules.DocumentUrlMaxLength} characters or fewer."));
    }
}