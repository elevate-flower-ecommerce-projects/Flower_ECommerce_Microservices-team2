using Cart_ServiceCart_Service.Common.ResultPattern;
using MediatR;

namespace Cart_ServiceCart_Service.Features.Cart.AddItem.NewFolder1;

public sealed record AddItemOrchestrator(
    long UserId,
    AddItemRequest Request
) : IRequest<RequestResult<CartSummaryResponse>>;
