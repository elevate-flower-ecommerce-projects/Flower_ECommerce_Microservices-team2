using Cart_ServiceCart_Service.Common.ResultPattern;
using MediatR;

namespace Cart_ServiceCart_Service.Features.Address.GetAddresses;

public static class GetAddressesEndpoint
{
    public static void MapGetAddressesEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/users/me/addresses", async (
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

            var result = await mediator.Send(new GetAddressesQuery(userId), cancellationToken);

            if (result.IsSuccess)
                return Results.Ok(EndpointResponse<IReadOnlyList<AddressResponse>>.Success(result.Data, result.Message));

            var response = EndpointResponse<IReadOnlyList<AddressResponse>>.Failure(result.ErrorCode, result.Message);

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
