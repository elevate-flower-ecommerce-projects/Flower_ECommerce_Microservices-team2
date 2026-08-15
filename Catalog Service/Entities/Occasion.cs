namespace Catalog_Service.Entities;

/// <summary>
/// An occasion a product can be offered for (e.g. Birthday, Wedding, Graduation).
/// </summary>
public class Occasion : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    // Navigation properties
    public ICollection<ProductOccasion> ProductOccasions { get; set; } = new List<ProductOccasion>();
}
