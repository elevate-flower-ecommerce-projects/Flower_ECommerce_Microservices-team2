using Cart_ServiceCart_Service.Common.ResultPattern;
using MediatR;

namespace Cart_ServiceCart_Service.Features.Cart.UpdateItemQuantity;

public sealed record UpdateItemQuantityCommand(
    long UserId,
    long CartItemId,
    UpdateItemQuantityRequest Request
) : IRequest<RequestResult<CartSummaryResponse>>;
