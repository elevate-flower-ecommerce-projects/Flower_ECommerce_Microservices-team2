using Catalog_Service.Common.ResultPattern;
using MediatR;
namespace Catalog_Service.Features.Catalog.GetProductCatalogeList.Query;

public record GetProductCatalogListQuery(
    int PageNumber,
    int PageSize,
    long? occasionId,
    long? categoryId,
    long? storeId
    ) : IRequest<RequestResult<PagedResult<ProductCatalogResultDto>>>;


public sealed record ProductCatalogResultDto (
    long ProductId,
    string ProductName,
    string? ImageUrl,
    decimal OriginalPrice,
    decimal? DiscountPercent,
    decimal? DiscountedPrice,
    int InStock
);
