using Cart_ServiceCart_Service.Common.Enums;
using Cart_ServiceCart_Service.Common.ResultPattern;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cart_ServiceCart_Service.Features.Address.GetAddressById;

public static class GetAddressByIdEndpoint
{
    public static void MapGetAddressByIdEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/users/me/addresses/{id:long}", async (
            [FromRoute] long id,
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

            var result = await mediator.Send(new GetAddressByIdQuery(userId, id), cancellationToken);

            if (result.IsSuccess)
            {
                return Results.Ok(EndpointResponse<AddressResponse>.Success(result.Data, result.Message));
            }

            var response = EndpointResponse<AddressResponse>.Failure(result.ErrorCode, result.Message);

            return result.ErrorCode switch
            {
                ErrorCode.NotFound => Results.NotFound(response),
                ErrorCode.Unauthorized => Results.Unauthorized(),
                _ => Results.BadRequest(response)
            };
        })
        .WithTags("Address")
        .RequireAuthorization();
    }
}
