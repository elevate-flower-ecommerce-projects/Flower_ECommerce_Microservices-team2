namespace Catalog_Service.Common.Enums;

/// <summary>
/// Kind of content a storefront section renders.
/// </summary>
public enum SectionType
{
    CategoryRail = 1,
    OccasionRail = 2,
    ProductRail = 3,
    Banner = 4,

    // Aliases for compatibility
    Category = 1,
    Occasion = 2,
    Product = 3
}
