using Catalog_Service.Common.ResultPattern;
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
        var categories = await mediator.Send(new GetCategoriesQuery(), cancellationToken);

        return Ok(EndpointResponse<IEnumerable<CategoryDto>>.Success(categories));
    }
}
