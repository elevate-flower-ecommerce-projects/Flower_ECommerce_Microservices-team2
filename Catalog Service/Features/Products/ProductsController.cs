using Catalog_Service.Common.Enums;
using Catalog_Service.Common.ResultPattern;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog_Service.Features.Products;

[ApiController]
[Route("products")]
public sealed class ProductsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(EndpointResponse<IEnumerable<ProductSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(EndpointResponse<IEnumerable<ProductSummaryDto>>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(EndpointResponse<IEnumerable<ProductSummaryDto>>), StatusCodes.Status410Gone)]
    public async Task<ActionResult<EndpointResponse<IEnumerable<ProductSummaryDto>>>> GetProducts(
        [FromQuery] long? categoryId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetProductsQuery(categoryId), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(EndpointResponse<IEnumerable<ProductSummaryDto>>.Success(
                result.Data,
                result.Message));
        }

        var response = EndpointResponse<IEnumerable<ProductSummaryDto>>.Failure(
            result.ErrorCode,
            result.Message);

        return result.ErrorCode switch
        {
            ErrorCode.CategoryNoLongerAvailable => StatusCode(StatusCodes.Status410Gone, response),
            ErrorCode.CategoryNotFound => NotFound(response),
            _ => BadRequest(response)
        };
    }
}
