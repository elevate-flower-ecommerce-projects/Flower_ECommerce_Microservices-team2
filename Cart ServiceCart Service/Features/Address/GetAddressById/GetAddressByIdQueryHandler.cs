using Cart_ServiceCart_Service.Common.BaseHandler;
using Cart_ServiceCart_Service.Common.Enums;
using Cart_ServiceCart_Service.Common.ResultPattern;
using Microsoft.EntityFrameworkCore;

namespace Cart_ServiceCart_Service.Features.Address.GetAddressById;

/// <summary>
/// Handles <see cref="GetAddressByIdQuery"/> by returning full address details
/// if the address exists and belongs to the authenticated user.
/// </summary>
public sealed class GetAddressByIdQueryHandler(BaseParameters baseParameters)
    : BaseHandler<GetAddressByIdQuery, RequestResult<AddressResponse>>(baseParameters)
{
    public override async Task<RequestResult<AddressResponse>> Handle(
        GetAddressByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request.UserId <= 0)
        {
            return RequestResult<AddressResponse>.Failure(ErrorCode.Unauthorized, "User must be authenticated.");
        }

        var address = await _context.Addresses
            .AsNoTracking()
            .Where(a => a.Id == request.AddressId && a.UserId == request.UserId)
            .Select(a => new AddressResponse(
                a.Id,
                a.RecipientName,
                a.RecipientPhone,
                a.City,
                a.Area,
                a.AddressLine,
                a.Label,
                a.Lat,
                a.Lng,
                a.IsDefault,
                a.StoreId,
                a.IsServiceable))
            .SingleOrDefaultAsync(cancellationToken);

        if (address is null)
        {
            return RequestResult<AddressResponse>.Failure(ErrorCode.NotFound, "Address not found.");
        }

        return RequestResult<AddressResponse>.Success(address);
    }
}
