using Cart_ServiceCart_Service.Common.ResultPattern;
using MediatR;

namespace Cart_ServiceCart_Service.Features.Address.UpdateAddress;

/// <summary>
/// MediatR command to update an existing delivery address for a user.
/// </summary>
public sealed record UpdateAddressCommand(long UserId, long AddressId, UpdateAddressRequest Request)
    : IRequest<RequestResult<AddressResponse>>;
