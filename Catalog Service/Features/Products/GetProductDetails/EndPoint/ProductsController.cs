using Catalog_Service.Common.ResultPattern;
using Catalog_Service.Features.Products.GetProductDetails.Dto;
using Catalog_Service.Features.Products.GetProductDetails.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Catalog_Service.Features.Products;

public sealed partial class ProductsController
{
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(EndpointResponse<ProductDetailsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(EndpointResponse<ProductDetailsDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EndpointResponse<ProductDetailsDto>>> GetProductDetails(
        [FromRoute] long id,
        [FromQuery] long? storeId = null)
    {
        var result = await mediator.Send(new GetProductDetailsQuery(id, storeId));

        if (result.IsSuccess)
        {
            return Ok(EndpointResponse<ProductDetailsDto>.Success(result.Data, result.Message));
        }

        return BadRequest(EndpointResponse<ProductDetailsDto>.Failure(result.ErrorCode, result.Message));
    }
}
