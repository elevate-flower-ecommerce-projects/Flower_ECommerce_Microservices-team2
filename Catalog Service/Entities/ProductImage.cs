namespace Catalog_Service.Entities;

/// <summary>
/// An image belonging to a product.
/// </summary>
public class ProductImage : BaseEntity
{
    public string Url { get; set; } = string.Empty;

    public long ProductId { get; set; }

    // Navigation properties
    public Product Product { get; set; } = null!;
}
