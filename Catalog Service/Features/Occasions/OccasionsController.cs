using Catalog_Service.Common.ResultPattern;
using Catalog_Service.Features.Occasions.Dto;
using Catalog_Service.Features.Occasions.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog_Service.Features.Occasions;

[ApiController]
[Route("occasions")]
public sealed class OccasionsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(EndpointResponse<PagedResult<OccasionDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<EndpointResponse<PagedResult<OccasionDto>>>> GetOccasions(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetAllOccasionsQuery(pageNumber, pageSize), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(EndpointResponse<PagedResult<OccasionDto>>.Success(result.Data, result.Message));
        }

        return BadRequest(EndpointResponse<PagedResult<OccasionDto>>.Failure(result.ErrorCode, result.Message));
    }

    [HttpGet("{occasionId:long}/products")]
    [ProducesResponseType(typeof(EndpointResponse<PagedResult<OccasionsProductsDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<EndpointResponse<PagedResult<OccasionsProductsDto>>>> GetOccasionProducts(
        [FromRoute] long occasionId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetOccasionProductsQuery(occasionId, pageNumber, pageSize), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(EndpointResponse<PagedResult<OccasionsProductsDto>>.Success(result.Data, result.Message));
        }

        return BadRequest(EndpointResponse<PagedResult<OccasionsProductsDto>>.Failure(result.ErrorCode, result.Message));
    }
}
