using System.Text.RegularExpressions;
using Cart_ServiceCart_Service.Common.BaseHandler;
using Cart_ServiceCart_Service.Common.Enums;
using Cart_ServiceCart_Service.Common.ResultPattern;
using Cart_ServiceCart_Service.Entities;
using Cart_ServiceCart_Service.Features.Address.Services;
using Microsoft.EntityFrameworkCore;

namespace Cart_ServiceCart_Service.Features.Address.CreateAddress;

/// <summary>
/// Handles <see cref="CreateAddressCommand"/> by creating a new delivery address,
/// auto-setting it as default if it's the user's first address,
/// and resolving the serving store via <see cref="IGeoLookupService"/>.
/// </summary>
public sealed class CreateAddressCommandHandler(
    BaseParameters baseParameters,
    IGeoLookupService geoLookupService)
    : BaseHandler<CreateAddressCommand, RequestResult<AddressResponse>>(baseParameters)
{
    private static readonly Regex EgyptianPhoneRegex = new(@"^(010|011|012|015)\d{8}$", RegexOptions.Compiled);

    public override async Task<RequestResult<AddressResponse>> Handle(
        CreateAddressCommand request,
        CancellationToken cancellationToken)
    {
        if (request.UserId <= 0)
        {
            return RequestResult<AddressResponse>.Failure(ErrorCode.Unauthorized, "User must be authenticated.");
        }

        var req = request.Request;
        if (req is null)
        {
            return RequestResult<AddressResponse>.Failure(ErrorCode.InvalidInput, "Address request body cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(req.RecipientName))
            return RequestResult<AddressResponse>.Failure(ErrorCode.InvalidInput, "Recipient name is required.");

        var phone = req.EffectivePhone.Trim();
        if (string.IsNullOrWhiteSpace(phone))
            return RequestResult<AddressResponse>.Failure(ErrorCode.InvalidInput, "Phone number is required.");

        if (!EgyptianPhoneRegex.IsMatch(phone))
            return RequestResult<AddressResponse>.Failure(
                ErrorCode.InvalidInput,
                "Phone number must start with 010, 011, 012 or 015 and have a total length of 11 digits.");

        if (string.IsNullOrWhiteSpace(req.AddressLine))
            return RequestResult<AddressResponse>.Failure(ErrorCode.InvalidInput, "Address line is required.");

        if (string.IsNullOrWhiteSpace(req.City))
            return RequestResult<AddressResponse>.Failure(ErrorCode.InvalidInput, "City is required.");

        if (string.IsNullOrWhiteSpace(req.Area))
            return RequestResult<AddressResponse>.Failure(ErrorCode.InvalidInput, "Area is required.");

        var hasExistingAddresses = await _context.Addresses
            .AnyAsync(a => a.UserId == request.UserId, cancellationToken);

        bool isDefault = !hasExistingAddresses || (req.IsDefault ?? false);

        // If newly created address is marked as default, clear any existing default address
        if (isDefault && hasExistingAddresses)
        {
            var existingDefaults = await _context.Addresses
                .Where(a => a.UserId == request.UserId && a.IsDefault)
                .ToListAsync(cancellationToken);

            foreach (var addr in existingDefaults)
            {
                addr.IsDefault = false;
                addr.UpdatedAt = DateTime.UtcNow;
                addr.UpdatedBy = request.UserId;
            }
        }

        var storeId = await geoLookupService.ResolveServingStoreIdAsync(
            req.City,
            req.Area,
            req.Lat,
            req.Lng,
            cancellationToken);

        bool isServiceable = storeId.HasValue;

        var address = new Entities.Address
        {
            Id = _snowflake.CreateId(),
            UserId = request.UserId,
            RecipientName = req.RecipientName.Trim(),
            RecipientPhone = phone,
            AddressLine = req.AddressLine.Trim(),
            City = req.City.Trim(),
            Area = req.Area.Trim(),
            Label = string.IsNullOrWhiteSpace(req.Label) ? null : req.Label.Trim(),
            Lat = req.Lat,
            Lng = req.Lng,
            IsDefault = isDefault,
            StoreId = storeId,
            IsServiceable = isServiceable,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = request.UserId
        };

        _context.Addresses.Add(address);
        await _context.SaveChangesAsync(cancellationToken);

        var response = new AddressResponse(
            address.Id,
            address.RecipientName,
            address.RecipientPhone,
            address.City,
            address.Area,
            address.AddressLine,
            address.Label,
            address.Lat,
            address.Lng,
            address.IsDefault,
            address.StoreId,
            address.IsServiceable);

        return RequestResult<AddressResponse>.Success(response, "Address created successfully.");
    }
}
