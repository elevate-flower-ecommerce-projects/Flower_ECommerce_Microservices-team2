using Catalog_Service.Common.Enums;

namespace Catalog_Service.Entities;

/// <summary>
/// A configurable storefront section used to arrange home catalog content.
/// Corresponds to SCRUM-85 Home Sections Schema.
/// </summary>
public class Section : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public int Order { get; set; }

    public SectionType Type { get; set; }

    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Type-specific JSON configuration:
    /// e.g. banner: { "imageUrl": "...", "deepLink": "...", "subtitle": "..." }
    /// e.g. product_rail: { "rule": "bestsellers", "categoryId": 10, "limit": 10, "viewAllDeepLink": "/products?..." }
    /// </summary>
    public string? ContentRefJson { get; set; }
}
