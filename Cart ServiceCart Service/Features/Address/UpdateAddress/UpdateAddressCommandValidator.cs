using FluentValidation;

namespace Cart_ServiceCart_Service.Features.Address.UpdateAddress;

/// <summary>
/// Validator for <see cref="UpdateAddressCommand"/> reusing the same validation rules as Add Address:
/// required fields and Egyptian mobile phone format matching customer registration.
/// </summary>
public sealed class UpdateAddressCommandValidator : AbstractValidator<UpdateAddressCommand>
{
    public UpdateAddressCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("User ID must be valid.");

        RuleFor(x => x.AddressId)
            .GreaterThan(0)
            .WithMessage("Address ID must be valid.");

        RuleFor(x => x.Request)
            .NotNull()
            .WithMessage("Address request body is required.");

        When(x => x.Request is not null, () =>
        {
            RuleFor(x => x.Request.RecipientName)
                .NotEmpty()
                .WithMessage("Recipient name is required.")
                .MaximumLength(150)
                .WithMessage("Recipient name cannot exceed 150 characters.");

            RuleFor(x => x.Request.EffectivePhone)
                .NotEmpty()
                .WithMessage("Phone number is required.")
                .Matches("^(010|011|012|015)\\d{8}$")
                .WithMessage("Phone number must start with 010, 011, 012 or 015 and have a total length of 11 digits.");

            RuleFor(x => x.Request.AddressLine)
                .NotEmpty()
                .WithMessage("Address line is required.")
                .MaximumLength(500)
                .WithMessage("Address line cannot exceed 500 characters.");

            RuleFor(x => x.Request.City)
                .NotEmpty()
                .WithMessage("City is required.")
                .MaximumLength(100)
                .WithMessage("City cannot exceed 100 characters.");

            RuleFor(x => x.Request.Area)
                .NotEmpty()
                .WithMessage("Area is required.")
                .MaximumLength(100)
                .WithMessage("Area cannot exceed 100 characters.");

            RuleFor(x => x.Request.Label)
                .MaximumLength(50)
                .When(x => !string.IsNullOrEmpty(x.Request.Label))
                .WithMessage("Label cannot exceed 50 characters.");

            RuleFor(x => x.Request.Lat)
                .InclusiveBetween(-90m, 90m)
                .When(x => x.Request.Lat.HasValue)
                .WithMessage("Latitude must be between -90 and 90.");

            RuleFor(x => x.Request.Lng)
                .InclusiveBetween(-180m, 180m)
                .When(x => x.Request.Lng.HasValue)
                .WithMessage("Longitude must be between -180 and 180.");
        });
    }
}
