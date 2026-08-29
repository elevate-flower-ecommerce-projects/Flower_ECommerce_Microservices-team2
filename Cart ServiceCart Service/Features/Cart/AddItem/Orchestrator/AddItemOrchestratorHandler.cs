using Cart_ServiceCart_Service.Common.BaseHandler;
using Cart_ServiceCart_Service.Common.ResultPattern;

namespace Cart_ServiceCart_Service.Features.Cart.AddItem.NewFolder1;

public sealed class AddItemOrchestratorHandler(BaseParameters baseParameters)
    : BaseHandler<AddItemOrchestrator, RequestResult<CartSummaryResponse>>(baseParameters)
{
    public override async Task<RequestResult<CartSummaryResponse>> Handle(
        AddItemOrchestrator request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new AddItemCommand(request.UserId, request.Request), cancellationToken);

        if (!result.IsSuccess)
            return RequestResult<CartSummaryResponse>.Failure(result.ErrorCode, result.Message);

        return RequestResult<CartSummaryResponse>.Success(result.Data, result.Message);
    }
}
