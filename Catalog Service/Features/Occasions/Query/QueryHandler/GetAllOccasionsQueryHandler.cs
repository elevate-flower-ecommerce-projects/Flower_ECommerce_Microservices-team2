using Catalog_Service.Common.BaseHandler;
using Catalog_Service.Common.ResultPattern;
using Catalog_Service.Entities;
using Catalog_Service.Features.Occasions.Dto;
using Catalog_Service.Features.Occasions.Query;

namespace Catalog_Service.Features.Occasions.Query.QueryHandler;

public sealed class GetAllOccasionsQueryHandler(BaseRequestParameters baseParameters)
    : BaseRequestHandler<GetAllOccasionsQuery, RequestResult<PagedResult<OccasionDto>>>(baseParameters)
{
    public override async Task<RequestResult<PagedResult<OccasionDto>>> Handle(
        GetAllOccasionsQuery request,
        CancellationToken cancellationToken)
    {
        var pagedResult = await _genericRepo.GetPagedAsync<Occasion, OccasionDto>(
            o => new OccasionDto(o.Id, o.Name, o.ImageUrl),
            request.PageNumber,
            request.PageSize,
            cancellationToken: cancellationToken);

        return RequestResult<PagedResult<OccasionDto>>.Success(pagedResult);
    }
}
