namespace Address___Store_Coverage_Service.Entities;

/// <summary>
/// Physical store that can cover one or more delivery areas.
/// </summary>
public class Store : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string PhysicalLocation { get; set; } = string.Empty;

    public string CoverageArea { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<StoreProduct> StoreProducts { get; set; } = new List<StoreProduct>();
}
