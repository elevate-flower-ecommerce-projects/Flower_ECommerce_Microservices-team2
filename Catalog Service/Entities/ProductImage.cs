namespace Catalog_Service.Entities;

/// <summary>
/// An image belonging to a product.
/// </summary>
public class ProductImage : BaseEntity
{
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Position of the image inside the product gallery (ascending). Existing rows default
    /// to 0, so ordering falls back to insertion order via <see cref="BaseEntity.Id"/>.
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>Alternative text for the image, used by the gallery for accessibility.</summary>
    public string? AltText { get; set; }

    public long ProductId { get; set; }

    // Navigation properties
    public Product Product { get; set; } = null!;
}
