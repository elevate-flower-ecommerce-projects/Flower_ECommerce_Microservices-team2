namespace Catalog_Service.Entities;

/// <summary>
/// Join table for the many-to-many relationship between Product and Occasion.
/// </summary>
public class ProductOccasion
{
    public long ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public long OccasionId { get; set; }
    public Occasion Occasion { get; set; } = null!;
}
