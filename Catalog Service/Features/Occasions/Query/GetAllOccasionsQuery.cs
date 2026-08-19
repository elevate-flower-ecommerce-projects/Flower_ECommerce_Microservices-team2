using Catalog_Service.Common.ResultPattern;
using Catalog_Service.Features.Occasions.Dto;
using MediatR;

namespace Catalog_Service.Features.Occasions.Query;

public sealed record GetAllOccasionsQuery(
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<RequestResult<PagedResult<OccasionDto>>>;
