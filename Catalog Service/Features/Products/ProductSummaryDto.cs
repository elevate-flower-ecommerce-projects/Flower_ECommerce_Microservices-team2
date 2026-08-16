using Catalog_Service.Common.Enums;

namespace Catalog_Service.Features.Products;

public sealed record ProductSummaryDto(
    long Id,
    string Name,
    decimal Price,
    decimal? DiscountPercentage,
    ProductStatus ProductStatus,
    int Quantity,
    string? ImageUrl,
    long CategoryId,
    string CategoryName);
