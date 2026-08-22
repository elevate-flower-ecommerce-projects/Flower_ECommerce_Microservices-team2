using Catalog_Service.Common.ResultPattern;
using Catalog_Service.Features.Catalog.GetProductCatalogeList.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog_Service.Features.Catalog.GetProductCatalogeList.Controller;

[ApiController]
[Route("Catalog")]
public sealed class CatalogController(IMediator mediator) : ControllerBase {
    /// <summary>
    /// Gets a paged list of product summaries with optional filters for category, occasion, and store.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(EndpointResponse<PagedResult<ProductCatalogResultDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(EndpointResponse<PagedResult<ProductCatalogResultDto>>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EndpointResponse<PagedResult<ProductCatalogResultDto>>>> GetFilteredProductsCatalgList(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] long? categoryId = null,
        [FromQuery] long? occasionId = null,
        [FromQuery] long? storeId = null,
        CancellationToken cancellationToken = default) {

        var query = new GetProductCatalogListQuery(
            pageNumber,
            pageSize,
            occasionId,
            categoryId,
            storeId);

        var result = await mediator.Send(query, cancellationToken);

        if (result.IsSuccess) {
            return Ok(EndpointResponse<PagedResult<ProductCatalogResultDto>>.Success(
                result.Data,
                result.Message));
        }

        return BadRequest(EndpointResponse<PagedResult<ProductCatalogResultDto>>.Failure(
            result.ErrorCode,
            result.Message));
    }

    /// <summary>
    /// Explicitly trigger or re-verify database seeding for all Catalog entities.
    /// </summary>
    [HttpPost("seed")]
    [HttpGet("seed")]
    [ProducesResponseType(typeof(EndpointResponse<Dictionary<string, int>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<EndpointResponse<Dictionary<string, int>>>> SeedCatalog(
        [FromServices] IServiceProvider serviceProvider)
    {
        var summary = await Catalog_Service.Data.Seeding.CatalogDataSeeder.SeedAsync(serviceProvider, force: true);
        return Ok(EndpointResponse<Dictionary<string, int>>.Success(summary, "Catalog data seeded successfully."));
    }
}
