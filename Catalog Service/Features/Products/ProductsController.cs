using Catalog_Service.Common.Enums;
using Catalog_Service.Common.ResultPattern;
using Catalog_Service.Features.Products.Dto;
using Catalog_Service.Features.Products.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog_Service.Features.Products;

[ApiController]
[Route("products")]
public sealed class ProductsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(EndpointResponse<PagedResult<ProductSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(EndpointResponse<PagedResult<ProductSummaryDto>>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(EndpointResponse<PagedResult<ProductSummaryDto>>), StatusCodes.Status410Gone)]
    public async Task<ActionResult<EndpointResponse<PagedResult<ProductSummaryDto>>>> GetProducts(
        [FromQuery] long? categoryId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetProductsQuery(categoryId, pageNumber, pageSize), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(EndpointResponse<PagedResult<ProductSummaryDto>>.Success(
                result.Data,
                result.Message));
        }

        var response = EndpointResponse<PagedResult<ProductSummaryDto>>.Failure(
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
