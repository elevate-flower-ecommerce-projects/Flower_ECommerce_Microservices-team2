using Catalog_Service.Common.ResultPattern;
using Catalog_Service.Features.Sections.Dto;
using Catalog_Service.Features.Sections.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog_Service.Features.Sections;

[ApiController]
[Route("sections")]
public sealed class SectionsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(EndpointResponse<IEnumerable<SectionDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<EndpointResponse<IEnumerable<SectionDto>>>> GetSections(
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetSectionsQuery(), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(EndpointResponse<IEnumerable<SectionDto>>.Success(result.Data, result.Message));
        }

        return BadRequest(EndpointResponse<IEnumerable<SectionDto>>.Failure(result.ErrorCode, result.Message));
    }
}
