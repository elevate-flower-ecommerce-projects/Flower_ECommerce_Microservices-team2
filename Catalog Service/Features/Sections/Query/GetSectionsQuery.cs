using Catalog_Service.Common.ResultPattern;
using Catalog_Service.Features.Sections.Dto;
using MediatR;

namespace Catalog_Service.Features.Sections.Query;

public sealed record GetSectionsQuery()
    : IRequest<RequestResult<IEnumerable<SectionDto>>>;
