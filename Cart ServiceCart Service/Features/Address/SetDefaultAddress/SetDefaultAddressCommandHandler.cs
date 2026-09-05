using Cart_ServiceCart_Service.Common.BaseHandler;
using Cart_ServiceCart_Service.Common.Enums;
using Cart_ServiceCart_Service.Common.ResultPattern;
using Microsoft.EntityFrameworkCore;

namespace Cart_ServiceCart_Service.Features.Address.SetDefaultAddress;

/// <summary>
/// Handles <see cref="SetDefaultAddressCommand"/> by un-setting any existing default address
/// for the user and setting the requested address as the default, guaranteeing exactly one default.
/// </summary>
public sealed class SetDefaultAddressCommandHandler(BaseParameters baseParameters)
    : BaseHandler<SetDefaultAddressCommand, RequestResult<AddressResponse>>(baseParameters)
{
    public override async Task<RequestResult<AddressResponse>> Handle(
        SetDefaultAddressCommand request,
        CancellationToken cancellationToken)
    {
        if (request.UserId <= 0)
        {
            return RequestResult<AddressResponse>.Failure(ErrorCode.Unauthorized, "User must be authenticated.");
        }

        if (request.AddressId <= 0)
        {
            return RequestResult<AddressResponse>.Failure(ErrorCode.InvalidInput, "Address ID must be greater than zero.");
        }

        // Find the target address belonging to this user
        var targetAddress = await _context.Addresses
            .SingleOrDefaultAsync(a => a.Id == request.AddressId && a.UserId == request.UserId, cancellationToken);

        if (targetAddress is null)
        {
            return RequestResult<AddressResponse>.Failure(ErrorCode.NotFound, "Address not found.");
        }

        // Find all other active default addresses for this user and un-set them
        var currentDefaults = await _context.Addresses
            .Where(a => a.UserId == request.UserId && a.Id != request.AddressId && a.IsDefault)
            .ToListAsync(cancellationToken);

        foreach (var addr in currentDefaults)
        {
            addr.IsDefault = false;
            addr.UpdatedAt = DateTime.UtcNow;
            addr.UpdatedBy = request.UserId;
        }

        // Set target address as default
        targetAddress.IsDefault = true;
        targetAddress.UpdatedAt = DateTime.UtcNow;
        targetAddress.UpdatedBy = request.UserId;

        await _context.SaveChangesAsync(cancellationToken);

        var response = new AddressResponse(
            targetAddress.Id,
            targetAddress.RecipientName,
            targetAddress.RecipientPhone,
            targetAddress.City,
            targetAddress.Area,
            targetAddress.AddressLine,
            targetAddress.Label,
            targetAddress.Lat,
            targetAddress.Lng,
            targetAddress.IsDefault,
            targetAddress.StoreId,
            targetAddress.IsServiceable);

        return RequestResult<AddressResponse>.Success(response, "Address set as default successfully.");
    }
}
