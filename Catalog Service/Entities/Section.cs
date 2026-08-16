using Catalog_Service.Common.Enums;

namespace Catalog_Service.Entities;

/// <summary>
/// A configurable storefront section used to arrange catalog content.
/// </summary>
public class Section : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public int Order { get; set; }

    public SectionType Type { get; set; }

    public bool IsEnabled { get; set; } = true;
}
