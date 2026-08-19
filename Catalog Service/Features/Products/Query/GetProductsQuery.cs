using Catalog_Service.Common.ResultPattern;
using Catalog_Service.Features.Products.Dto;
using MediatR;

namespace Catalog_Service.Features.Products.Query;

public sealed record GetProductsQuery(
    long? CategoryId = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<RequestResult<PagedResult<ProductSummaryDto>>>;
