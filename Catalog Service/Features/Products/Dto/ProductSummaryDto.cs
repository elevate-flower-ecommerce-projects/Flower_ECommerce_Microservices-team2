using Catalog_Service.Common.Enums;

namespace Catalog_Service.Features.Products.Dto;

public sealed record ProductSummaryDto(
    long Id,
    string Name,
    decimal Price,
    decimal? DiscountPercentage,
    ProductStatus Status,
    int Quantity,
    string? ImageUrl,
    long CategoryId,
    string CategoryName);
