using Catalog_Service.Common.Enums;
using Catalog_Service.Features.Occasions.Dto;

namespace Catalog_Service.Features.Products.GetProductDetails.Dto;

public sealed record ProductDetailsDto(
    long Id,
    string Name,
    string? Description,
    ProductStatus Status,
    long CategoryId,
    string CategoryName,
    IReadOnlyList<ProductGalleryImageDto> Images,
    ProductPricingDto Pricing,
    ProductAvailabilityDto Availability,
    IReadOnlyList<ProductIncludeDto> Includes,
    IReadOnlyList<OccasionDto> Occasions);

public sealed record ProductGalleryImageDto(
    long Id,
    string Url,
    int DisplayOrder,
    string? AltText,
    bool IsPrimary);

public sealed record ProductPricingDto(
    decimal OriginalPrice,
    decimal? DiscountPercentage,
    decimal? DiscountedPrice,
    decimal EffectivePrice,
    bool HasActiveDiscount,
    DateTime? DiscountStartAt,
    DateTime? DiscountEndAt,
    bool IsStoreScoped);

public sealed record ProductAvailabilityDto(
    long? StoreId,
    string? StoreName,
    bool IsStoreResolved,
    bool RequiresStoreSelection,
    AvailabilityStatus Status,
    bool IsAvailable,
    bool CanAddToCart,
    int? AvailableStock,
    int? MaxOrderQuantity,
    string Message);

public sealed record ProductIncludeDto(
    long Id,
    string Item,
    int Quantity,
    int DisplayOrder);
