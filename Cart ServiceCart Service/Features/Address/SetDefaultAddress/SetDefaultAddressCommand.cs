using Cart_ServiceCart_Service.Common.Interface;
using Cart_ServiceCart_Service.Common.ResultPattern;
using MediatR;

namespace Cart_ServiceCart_Service.Features.Address.SetDefaultAddress;

/// <summary>
/// MediatR command to set an address as default for the authenticated user,
/// transactional across previous and new defaults.
/// </summary>
public sealed record SetDefaultAddressCommand(long UserId, long AddressId)
    : ICommand<RequestResult<AddressResponse>>;
