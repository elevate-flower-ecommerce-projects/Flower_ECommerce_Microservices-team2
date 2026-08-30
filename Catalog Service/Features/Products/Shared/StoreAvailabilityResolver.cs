using Catalog_Service.Common.Enums;
using Catalog_Service.Common.Helpers;
using Catalog_Service.Common.Services;
using Catalog_Service.Data;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.Features.Products.Shared;

public sealed class StoreAvailabilityResolver(
    CatalogServiceDbContext context,
    CancellationTokenCapture cancellationTokenCapture) : IStoreAvailabilityResolver
{
    private readonly CatalogServiceDbContext _context = context;
    private readonly CancellationTokenCapture _cancellationTokenCapture = cancellationTokenCapture;

    public async Task<StoreAvailabilityResolution> ResolveAsync(
        long productId,
        long? storeId,
        ProductStatus productStatus)
    {
        var cancellationToken = _cancellationTokenCapture.Token;

        if (productStatus == ProductStatus.Discontinued)
        {
            return Unavailable(storeId, storeName: null, isStoreResolved: false, AvailabilityStatus.Discontinued);
        }

        if (storeId is null or <= 0)
        {
            return new StoreAvailabilityResolution(
                StoreId: null,
                StoreName: null,
                IsStoreResolved: false,
                RequiresStoreSelection: true,
                Status: AvailabilityStatus.StoreNotSelected,
                IsAvailable: false,
                CanAddToCart: false,
                AvailableStock: null,
                MaxOrderQuantity: null,
                PriceOverride: null,
                Message: AvailabilityStatus.StoreNotSelected.GetDescription());
        }

        var store = await _context.Stores
            .AsNoTracking()
            .Where(s => s.Id == storeId.Value && s.IsActive)
            .Select(s => new { s.Id, s.Name })
            .FirstOrDefaultAsync(cancellationToken);

        if (store is null)
        {
            return Unavailable(storeId, storeName: null, isStoreResolved: false, AvailabilityStatus.StoreUnavailable);
        }

        var inventory = await _context.ProductStoreInventories
            .AsNoTracking()
            .Where(psi => psi.ProductId == productId && psi.StoreId == store.Id)
            .Select(psi => new { psi.StockQuantity, psi.MaxOrderQuantity, psi.PriceOverride, psi.IsActive })
            .FirstOrDefaultAsync(cancellationToken);

        if (inventory is null)
        {
            return Unavailable(store.Id, store.Name, isStoreResolved: true, AvailabilityStatus.NotCarried);
        }

        if (!inventory.IsActive || inventory.StockQuantity <= 0)
        {
            return Unavailable(store.Id, store.Name, isStoreResolved: true, AvailabilityStatus.OutOfStock)
                with { PriceOverride = inventory.PriceOverride };
        }

        var availableStock = inventory.StockQuantity;
        var maxOrderQuantity = inventory.MaxOrderQuantity is > 0
            ? Math.Min(inventory.MaxOrderQuantity.Value, availableStock)
            : availableStock;

        return new StoreAvailabilityResolution(
            StoreId: store.Id,
            StoreName: store.Name,
            IsStoreResolved: true,
            RequiresStoreSelection: false,
            Status: AvailabilityStatus.InStock,
            IsAvailable: true,
            CanAddToCart: true,
            AvailableStock: availableStock,
            MaxOrderQuantity: maxOrderQuantity,
            PriceOverride: inventory.PriceOverride,
            Message: AvailabilityStatus.InStock.GetDescription());
    }

    private static StoreAvailabilityResolution Unavailable(
        long? storeId,
        string? storeName,
        bool isStoreResolved,
        AvailabilityStatus status) =>
        new(
            StoreId: storeId,
            StoreName: storeName,
            IsStoreResolved: isStoreResolved,
            RequiresStoreSelection: false,
            Status: status,
            IsAvailable: false,
            CanAddToCart: false,
            AvailableStock: 0,
            MaxOrderQuantity: 0,
            PriceOverride: null,
            Message: status.GetDescription());
}
