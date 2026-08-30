namespace Catalog_Service.Entities;

/// <summary>
/// Stock (and optional price override) of one product at one store.
/// This is what makes availability and price store-scoped: a product with no row here
/// for the resolved store is simply not carried by that store.
/// </summary>
public class ProductStoreInventory : BaseEntity
{
    public long ProductId { get; set; }

    public long StoreId { get; set; }

    /// <summary>Units currently on hand at this store. Never negative.</summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// Optional per-store cap on a single order line, independent of stock.
    /// The quantity selector uses min(StockQuantity, MaxOrderQuantity).
    /// </summary>
    public int? MaxOrderQuantity { get; set; }

    /// <summary>
    /// Optional store-specific price. When set it replaces <see cref="Product.Price"/>
    /// as the base the discount percentage is applied to.
    /// </summary>
    public decimal? PriceOverride { get; set; }

    /// <summary>False when the store has delisted the product without deleting its stock record.</summary>
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Product Product { get; set; } = null!;
    public Store Store { get; set; } = null!;
}
