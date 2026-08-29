using Cart_ServiceCart_Service.Common.BaseHandler;
using Cart_ServiceCart_Service.Common.ResultPattern;

namespace Cart_ServiceCart_Service.Features.Cart.UpdateItemQuantity.NewFolder1;

public sealed class UpdateItemQuantityOrchestratorHandler(BaseParameters baseParameters)
    : BaseHandler<UpdateItemQuantityOrchestrator, RequestResult<CartSummaryResponse>>(baseParameters)
{
    public override async Task<RequestResult<CartSummaryResponse>> Handle(
        UpdateItemQuantityOrchestrator request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateItemQuantityCommand(request.UserId, request.CartItemId, request.Request),
            cancellationToken);

        if (!result.IsSuccess)
            return RequestResult<CartSummaryResponse>.Failure(result.ErrorCode, result.Message);

        return RequestResult<CartSummaryResponse>.Success(result.Data, result.Message);
    }
}
