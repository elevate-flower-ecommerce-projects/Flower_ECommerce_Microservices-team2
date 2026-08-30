namespace Catalog_Service.Entities;

/// <summary>
/// Local read-model of a store owned by the Address &amp; Store Coverage service.
/// The Catalog service keeps its own copy so product availability can be resolved
/// for a given store without a synchronous cross-service call; it is never the
/// source of truth for store master data.
/// </summary>
public class Store : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string? CoverageArea { get; set; }

    /// <summary>
    /// False when the store is temporarily not serving orders. An inactive store never
    /// resolves, so products are reported as unavailable rather than possibly-wrong.
    /// </summary>
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<ProductStoreInventory> ProductInventories { get; set; } = new List<ProductStoreInventory>();
}
