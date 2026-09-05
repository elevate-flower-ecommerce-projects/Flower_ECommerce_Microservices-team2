using Cart_ServiceCart_Service.Common.ResultPattern;
using MediatR;

namespace Cart_ServiceCart_Service.Features.Address.DeleteAddress;

/// <summary>
/// MediatR command to delete a delivery address for a user.
/// </summary>
public sealed record DeleteAddressCommand(long UserId, long AddressId)
    : IRequest<RequestResult<bool>>;
