using Cart_ServiceCart_Service.Common.ResultPattern;
using MediatR;

namespace Cart_ServiceCart_Service.Features.Address.GetAddressById;

/// <summary>
/// MediatR query to fetch a specific address by its ID for the authenticated user.
/// </summary>
public sealed record GetAddressByIdQuery(long UserId, long AddressId)
    : IRequest<RequestResult<AddressResponse>>;
