using Catalog_Service.Common.BaseHandler;
using Catalog_Service.Common.ResultPattern;
using Catalog_Service.Features.Occasions.Dto;
using Catalog_Service.Features.Occasions.Query;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.Features.Occasions.Query.QueryHandler;

public sealed class GetAllOccasionsQueryHandler(BaseRequestParameters baseParameters)
    : BaseRequestHandler<GetAllOccasionsQuery, RequestResult<IEnumerable<OccasionDto>>>(baseParameters)
{
    public override async Task<RequestResult<IEnumerable<OccasionDto>>> Handle(
        GetAllOccasionsQuery request,
        CancellationToken cancellationToken)
    {
        var occasions = await _context.Occasions
            .OrderBy(occasion => occasion.Name)
            .ThenBy(occasion => occasion.Id)
            .Select(occasion => new OccasionDto(
                occasion.Id,
                occasion.Name,
                occasion.ImageUrl))
            .ToListAsync(cancellationToken);

        return RequestResult<IEnumerable<OccasionDto>>.Success(occasions);
    }
}
