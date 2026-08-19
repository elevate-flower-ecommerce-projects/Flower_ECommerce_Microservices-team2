using Catalog_Service.Common.BaseHandler;
using Catalog_Service.Common.ResultPattern;
using Catalog_Service.Features.Sections.Dto;
using Catalog_Service.Features.Sections.Query;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.Features.Sections.Query.QueryHandler;

public sealed class GetSectionsQueryHandler(BaseRequestParameters baseParameters)
    : BaseRequestHandler<GetSectionsQuery, RequestResult<IEnumerable<SectionDto>>>(baseParameters)
{
    public override async Task<RequestResult<IEnumerable<SectionDto>>> Handle(
        GetSectionsQuery request,
        CancellationToken cancellationToken)
    {
        var sections = await _context.Sections
            .Where(s => s.IsEnabled)
            .OrderBy(s => s.Order)
            .Select(s => new SectionDto(
                s.Id,
                s.Title,
                s.Order,
                s.Type,
                s.IsEnabled,
                s.ContentRefJson))
            .ToListAsync(cancellationToken);

        return RequestResult<IEnumerable<SectionDto>>.Success(sections);
    }
}
