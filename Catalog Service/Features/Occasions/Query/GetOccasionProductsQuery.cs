using Catalog_Service.Common.ResultPattern;
using Catalog_Service.Features.Occasions.Dto;
using MediatR;

namespace Catalog_Service.Features.Occasions.Query;

public sealed record GetOccasionProductsQuery(
    long OccasionId,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<RequestResult<PagedResult<OccasionsProductsDto>>>;
