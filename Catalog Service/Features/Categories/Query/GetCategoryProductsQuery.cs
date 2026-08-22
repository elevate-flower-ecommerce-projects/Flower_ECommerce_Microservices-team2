using Catalog_Service.Common.ResultPattern;
using Catalog_Service.Features.Products.Dto;
using MediatR;

namespace Catalog_Service.Features.Categories.Query;

public sealed record GetCategoryProductsQuery(
    long CategoryId,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<RequestResult<PagedResult<ProductSummaryDto>>>;
