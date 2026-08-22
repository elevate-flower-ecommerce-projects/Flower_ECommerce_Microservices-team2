using Catalog_Service.Common.Enums;

namespace Catalog_Service.Entities;

/// <summary>
/// A sellable catalog item belonging to a single category.
/// </summary>
public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public decimal? DiscountPercentage { get; set; }

    public DateTime? DiscountStartAt { get; set; }

    public DateTime? DiscountEndAt { get; set; }

    public ProductStatus ProductStatus { get; set; } = ProductStatus.Draft;

    public int Quantity { get; set; }

    public string? Description { get; set; }

    public bool IsArchived { get; set; } = false;

    public long CategoryId { get; set; }

    // Navigation properties
    public Category Category { get; set; } = null!;
    public IEnumerable<ProductImage> Images { get; set; } 
    public IEnumerable<ProductOccasion> ProductOccasions { get; set; }  
}
