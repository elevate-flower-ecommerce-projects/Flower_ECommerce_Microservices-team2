using Cart_ServiceCart_Service.Common.ResultPattern;
using MediatR;

namespace Cart_ServiceCart_Service.Features.Cart.AddItem;

public sealed record AddItemCommand(
    long UserId,
    AddItemRequest Request
) : IRequest<RequestResult<CartSummaryResponse>>;
