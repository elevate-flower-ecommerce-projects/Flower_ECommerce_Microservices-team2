using System.Text.RegularExpressions;
using Cart_ServiceCart_Service.Common.BaseHandler;
using Cart_ServiceCart_Service.Common.Enums;
using Cart_ServiceCart_Service.Common.ResultPattern;
using Cart_ServiceCart_Service.Features.Address.Services;
using Microsoft.EntityFrameworkCore;

namespace Cart_ServiceCart_Service.Features.Address.UpdateAddress;

/// <summary>
/// Handles <see cref="UpdateAddressCommand"/> by updating an existing delivery address,
/// re-resolving the serving store if location/pin changed, and handling default reassignment.
/// </summary>
public sealed class UpdateAddressCommandHandler(
    BaseParameters baseParameters,
    IGeoLookupService geoLookupService)
    : BaseHandler<UpdateAddressCommand, RequestResult<AddressResponse>>(baseParameters)
{
    private static readonly Regex EgyptianPhoneRegex = new(@"^(010|011|012|015)\d{8}$", RegexOptions.Compiled);

    public override async Task<RequestResult<AddressResponse>> Handle(
        UpdateAddressCommand request,
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

        // Defensive validation for required fields
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

        // Fetch existing address
        var address = await _context.Addresses
            .SingleOrDefaultAsync(a => a.Id == request.AddressId && a.UserId == request.UserId, cancellationToken);

        if (address is null)
        {
            return RequestResult<AddressResponse>.Failure(ErrorCode.NotFound, "Address not found.");
        }

        // 1. Re-resolve serving store if location fields changed
        // "Saving an edit re-resolves the serving store if the address line/city/area/pin changed"
        bool locationChanged =
            !string.Equals(address.City, req.City.Trim(), StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(address.Area, req.Area.Trim(), StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(address.AddressLine, req.AddressLine.Trim(), StringComparison.OrdinalIgnoreCase) ||
            address.Lat != req.Lat ||
            address.Lng != req.Lng;

        if (locationChanged)
        {
            var storeId = await geoLookupService.ResolveServingStoreIdAsync(
                req.City.Trim(),
                req.Area.Trim(),
                req.Lat,
                req.Lng,
                cancellationToken);

            address.StoreId = storeId;
            address.IsServiceable = storeId.HasValue;
        }

        // 2. Default address handling
        if (req.IsDefault.HasValue)
        {
            if (req.IsDefault.Value && !address.IsDefault)
            {
                var existingDefaults = await _context.Addresses
                    .Where(a => a.UserId == request.UserId && a.Id != address.Id && a.IsDefault)
                    .ToListAsync(cancellationToken);

                foreach (var addr in existingDefaults)
                {
                    addr.IsDefault = false;
                    addr.UpdatedAt = DateTime.UtcNow;
                    addr.UpdatedBy = request.UserId;
                }

                address.IsDefault = true;
            }
            else if (!req.IsDefault.Value && address.IsDefault)
            {
                address.IsDefault = false;
            }
        }

        // 3. Update entity fields
        address.RecipientName = req.RecipientName.Trim();
        address.RecipientPhone = phone;
        address.AddressLine = req.AddressLine.Trim();
        address.City = req.City.Trim();
        address.Area = req.Area.Trim();
        address.Label = string.IsNullOrWhiteSpace(req.Label) ? null : req.Label.Trim();
        address.Lat = req.Lat;
        address.Lng = req.Lng;
        address.UpdatedAt = DateTime.UtcNow;
        address.UpdatedBy = request.UserId;

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

        return RequestResult<AddressResponse>.Success(response, "Address updated successfully.");
    }
}
