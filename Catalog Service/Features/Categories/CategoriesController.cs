using Catalog_Service.Common.Enums;
using Catalog_Service.Common.ResultPattern;
using Catalog_Service.Features.Categories.Dto;
using Catalog_Service.Features.Categories.Query;
using Catalog_Service.Features.Products.GetProducts.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog_Service.Features.Categories;

[ApiController]
[Route("categories")]
public sealed class CategoriesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(EndpointResponse<IEnumerable<CategoryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<EndpointResponse<IEnumerable<CategoryDto>>>> GetCategories(
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetCategoriesQuery(), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(EndpointResponse<IEnumerable<CategoryDto>>.Success(result.Data, result.Message));
        }

        return BadRequest(EndpointResponse<IEnumerable<CategoryDto>>.Failure(result.ErrorCode, result.Message));
    }

    [HttpGet("{categoryId:long}/products")]
    [ProducesResponseType(typeof(EndpointResponse<PagedResult<ProductSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(EndpointResponse<PagedResult<ProductSummaryDto>>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(EndpointResponse<PagedResult<ProductSummaryDto>>), StatusCodes.Status410Gone)]
    public async Task<ActionResult<EndpointResponse<PagedResult<ProductSummaryDto>>>> GetCategoryProducts(
        [FromRoute] long categoryId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetCategoryProductsQuery(categoryId, pageNumber, pageSize), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(EndpointResponse<PagedResult<ProductSummaryDto>>.Success(result.Data, result.Message));
        }

        var response = EndpointResponse<PagedResult<ProductSummaryDto>>.Failure(result.ErrorCode, result.Message);

        return result.ErrorCode switch
        {
            ErrorCode.CategoryNoLongerAvailable => StatusCode(StatusCodes.Status410Gone, response),
            ErrorCode.CategoryNotFound => NotFound(response),
            _ => BadRequest(response)
        };
    }
}
