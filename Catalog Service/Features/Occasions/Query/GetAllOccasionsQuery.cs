using Catalog_Service.Common.ResultPattern;
using Catalog_Service.Features.Occasions.Dto;
using MediatR;

namespace Catalog_Service.Features.Occasions.Query;

public sealed record GetAllOccasionsQuery()
    : IRequest<RequestResult<IEnumerable<OccasionDto>>>;
