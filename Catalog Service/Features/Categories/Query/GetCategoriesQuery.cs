using Catalog_Service.Common.ResultPattern;
using Catalog_Service.Features.Categories.Dto;
using MediatR;

namespace Catalog_Service.Features.Categories.Query;

public sealed record GetCategoriesQuery()
    : IRequest<RequestResult<IEnumerable<CategoryDto>>>;
