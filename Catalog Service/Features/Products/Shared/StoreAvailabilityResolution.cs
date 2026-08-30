using Catalog_Service.Common.Enums;

namespace Catalog_Service.Features.Products.Shared;

public sealed record StoreAvailabilityResolution(
    long? StoreId,
    string? StoreName,
    bool IsStoreResolved,
    bool RequiresStoreSelection,
    AvailabilityStatus Status,
    bool IsAvailable,
    bool CanAddToCart,
    int? AvailableStock,
    int? MaxOrderQuantity,
    decimal? PriceOverride,
    string Message);
