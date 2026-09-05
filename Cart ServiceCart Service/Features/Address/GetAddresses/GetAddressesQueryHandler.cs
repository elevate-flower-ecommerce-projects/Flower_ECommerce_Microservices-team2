using Cart_ServiceCart_Service.Common.BaseHandler;
using Cart_ServiceCart_Service.Common.ResultPattern;
using Microsoft.EntityFrameworkCore;

namespace Cart_ServiceCart_Service.Features.Address.GetAddresses;

/// <summary>
/// Handles <see cref="GetAddressesQuery"/> by fetching the user's saved addresses
/// ordered with the default address first, then by most recently created.
/// </summary>
public sealed class GetAddressesQueryHandler(BaseParameters baseParameters)
    : BaseHandler<GetAddressesQuery, RequestResult<IReadOnlyList<AddressResponse>>>(baseParameters)
{
    public override async Task<RequestResult<IReadOnlyList<AddressResponse>>> Handle(
        GetAddressesQuery request,
        CancellationToken cancellationToken)
    {
        var addresses = await _context.Addresses
            .AsNoTracking()
            .Where(a => a.UserId == request.UserId)
            .OrderByDescending(a => a.IsDefault)
            .ThenByDescending(a => a.CreatedAt)
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
            .ToListAsync(cancellationToken);

        return RequestResult<IReadOnlyList<AddressResponse>>.Success(addresses);
    }
}
