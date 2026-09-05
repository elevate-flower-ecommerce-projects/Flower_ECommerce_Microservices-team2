using Cart_ServiceCart_Service.Common.ResultPattern;
using MediatR;

namespace Cart_ServiceCart_Service.Features.Address.GetAddresses;

/// <summary>
/// Query to retrieve all saved addresses for a given user,
/// ordered default-first then by most recently created.
/// </summary>
public sealed record GetAddressesQuery(long UserId)
    : IRequest<RequestResult<IReadOnlyList<AddressResponse>>>;
