using Catalog_Service.Common.ResultPattern;
using Catalog_Service.Features.Products.GetProducts.Dto;
using Catalog_Service.Features.Products.GetProducts.Queries;
using Microsoft.AspNetCore.Mvc;

// Part of the ProductsController declared in Features/Products/ProductsController.cs — partial
// parts have to share that namespace, which is why this file does not follow its folder.
namespace Catalog_Service.Features.Products;

public sealed partial class ProductsController
{

    [HttpGet]
    [ProducesResponseType(typeof(EndpointResponse<PagedResult<ProductSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(EndpointResponse<PagedResult<ProductSummaryDto>>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EndpointResponse<PagedResult<ProductSummaryDto>>>> GetProducts(
        [FromQuery] long? categoryId,
        [FromQuery] long? occasionId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await mediator.Send(new GetProductsQuery(categoryId, occasionId, pageNumber, pageSize));

        if (result.IsSuccess)
        {
            return Ok(EndpointResponse<PagedResult<ProductSummaryDto>>.Success(
                result.Data,
                result.Message));
        }

        return BadRequest(EndpointResponse<PagedResult<ProductSummaryDto>>.Failure(
            result.ErrorCode,
            result.Message));
    }
}
