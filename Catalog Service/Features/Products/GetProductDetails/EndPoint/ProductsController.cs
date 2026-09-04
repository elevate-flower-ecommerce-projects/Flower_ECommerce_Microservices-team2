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
    [ProducesResponseType(typeof(EndpointResponse<ProductDetailsDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(EndpointResponse<ProductDetailsDto>), StatusCodes.Status410Gone)]
    public async Task<ActionResult<EndpointResponse<ProductDetailsDto>>> GetProductDetails(
        [FromRoute] long id,
        [FromQuery] long? storeId = null,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetProductDetailsQuery(id, storeId), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(EndpointResponse<ProductDetailsDto>.Success(result.Data, result.Message));
        }

        var response = EndpointResponse<ProductDetailsDto>.Failure(result.ErrorCode, result.Message);

        return result.ErrorCode switch
        {
            Catalog_Service.Common.Enums.ErrorCode.ProductNoLongerAvailable => StatusCode(StatusCodes.Status410Gone, response),
            Catalog_Service.Common.Enums.ErrorCode.ProductNotFound => NotFound(response),
            _ => BadRequest(response)
        };
    }
}
