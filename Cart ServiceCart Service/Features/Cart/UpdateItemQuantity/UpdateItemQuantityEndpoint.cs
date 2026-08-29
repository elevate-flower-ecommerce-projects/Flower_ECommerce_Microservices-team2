using Cart_ServiceCart_Service.Common.ResultPattern;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cart_ServiceCart_Service.Features.Cart.UpdateItemQuantity;

public static class UpdateItemQuantityEndpoint
{
    public static void MapUpdateItemQuantityEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPatch("/cart/items/{cartItemId:long}", async (
            long cartItemId,
            [FromBody] UpdateItemQuantityRequest request,
            HttpContext httpContext,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var userIdClaim = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                ?? httpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

            if (!long.TryParse(userIdClaim?.Value, out var userId) || userId <= 0)
            {
                return Results.Unauthorized();
            }

            var result = await mediator.Send(new UpdateItemQuantityOrchestrator(userId, cartItemId, request), cancellationToken);

            if (result.IsSuccess)
                return Results.Ok(EndpointResponse<CartSummaryResponse>.Success(result.Data, result.Message));

            var response = EndpointResponse<CartSummaryResponse>.Failure(result.ErrorCode, result.Message);

            return result.ErrorCode switch
            {
                ErrorCode.InvalidInput => Results.BadRequest(response),
                ErrorCode.NotFound => Results.NotFound(response),
                ErrorCode.Conflict => Results.Conflict(response),
                ErrorCode.ServiceUnavailable => Results.Json(response, statusCode: StatusCodes.Status503ServiceUnavailable),
                ErrorCode.Unauthorized => Results.Unauthorized(),
                _ => Results.BadRequest(response)
            };
        })
        .WithTags("Cart")
        .AllowAnonymous();
    }
}
