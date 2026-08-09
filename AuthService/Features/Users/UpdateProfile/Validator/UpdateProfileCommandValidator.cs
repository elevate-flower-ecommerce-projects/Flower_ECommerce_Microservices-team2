using AuthService.Common.Validation;
using FluentValidation;

namespace AuthService.Features.Users.UpdateProfile;

public sealed class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(command => command)
            .Must(command => command.FullName is not null || command.Email is not null ||
                             command.Phone is not null || command.Gender is not null ||
                             command.Document is not null)
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

        When(command => command.Phone is not null, () =>
            RuleFor(command => command.Phone)
                .Matches(UserValidationRules.PhonePattern)
                .WithMessage("phone must be a valid international phone number.")
                .MaximumLength(UserValidationRules.PhoneMaxLength)
                .WithMessage($"phone must be {UserValidationRules.PhoneMaxLength} characters or fewer."));

        When(command => command.Gender is not null, () =>
            RuleFor(command => command.Gender)
                .IsInEnum()
                .WithMessage("gender is invalid."));

        When(command => command.Document is not null, () =>
        {
            RuleFor(command => command.Document!.DocumentUrl)
                .Must(value => Uri.TryCreate(value, UriKind.Absolute, out var uri) &&
                               (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                .WithMessage("document.documentUrl must be a served HTTP or HTTPS URL.")
                .MaximumLength(UserValidationRules.DocumentUrlMaxLength)
                .WithMessage($"document.documentUrl must be {UserValidationRules.DocumentUrlMaxLength} characters or fewer.");

            RuleFor(command => command.Document!.DocumentType)
                .Must(value => !string.IsNullOrWhiteSpace(value))
                .WithMessage("document.documentType is required.")
                .MaximumLength(UserValidationRules.DocumentTypeMaxLength)
                .WithMessage($"document.documentType must be {UserValidationRules.DocumentTypeMaxLength} characters or fewer.");

            RuleFor(command => command.Document!.DocumentSize)
                .GreaterThan(0)
                .WithMessage("document.documentSize must be greater than zero.");
        });
    }
}