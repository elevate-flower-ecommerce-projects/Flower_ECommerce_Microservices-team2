using Cart_ServiceCart_Service.Common.ResultPattern;
using MediatR;

namespace Cart_ServiceCart_Service.Features.Address.CreateAddress;

/// <summary>
/// MediatR command to create a new delivery address for a user.
/// </summary>
public sealed record CreateAddressCommand(long UserId, CreateAddressRequest Request)
    : IRequest<RequestResult<AddressResponse>>;
