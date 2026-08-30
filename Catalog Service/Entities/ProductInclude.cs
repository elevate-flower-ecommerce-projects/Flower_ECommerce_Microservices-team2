namespace Catalog_Service.Entities;

/// <summary>
/// A single line of a product's "what's included" list
/// (e.g. "1 x hand-tied bouquet", "1 x ceramic vase", "Free greeting card").
/// </summary>
public class ProductInclude : BaseEntity
{
    /// <summary>Human readable item description shown on the product details screen.</summary>
    public string Item { get; set; } = string.Empty;

    /// <summary>How many of this item are included. Defaults to a single unit.</summary>
    public int Quantity { get; set; } = 1;

    /// <summary>Order the item appears in within the list (ascending).</summary>
    public int DisplayOrder { get; set; }

    public long ProductId { get; set; }

    // Navigation properties
    public Product Product { get; set; } = null!;
}
