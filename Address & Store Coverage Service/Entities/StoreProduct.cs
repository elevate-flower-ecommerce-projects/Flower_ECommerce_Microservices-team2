namespace Address___Store_Coverage_Service.Entities;

/// <summary>
/// Join table for products available from a store.
/// </summary>
public class StoreProduct
{
    public long StoreId { get; set; }

    public long ProductId { get; set; }

    // Navigation properties
    public Store Store { get; set; } = null!;
}
