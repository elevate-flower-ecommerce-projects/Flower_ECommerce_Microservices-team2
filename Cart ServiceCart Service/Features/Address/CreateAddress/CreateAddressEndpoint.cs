using Cart_ServiceCart_Service.Common.Enums;
using Cart_ServiceCart_Service.Common.ResultPattern;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cart_ServiceCart_Service.Features.Address.CreateAddress;

public static class CreateAddressEndpoint
{
    public static void MapCreateAddressEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/users/me/addresses", async (
            [FromBody] CreateAddressRequest request,
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

            var result = await mediator.Send(new CreateAddressCommand(userId, request), cancellationToken);

            if (result.IsSuccess)
            {
                return Results.Created(
                    $"/users/me/addresses/{result.Data.Id}",
                    EndpointResponse<AddressResponse>.Success(result.Data, result.Message));
            }

            var response = EndpointResponse<AddressResponse>.Failure(result.ErrorCode, result.Message);

            return result.ErrorCode switch
            {
                ErrorCode.InvalidInput => Results.BadRequest(response),
                ErrorCode.Unauthorized => Results.Unauthorized(),
                ErrorCode.NotFound => Results.NotFound(response),
                ErrorCode.Conflict => Results.Conflict(response),
                _ => Results.BadRequest(response)
            };
        })
        .WithTags("Address")
        .RequireAuthorization();
    }
}
