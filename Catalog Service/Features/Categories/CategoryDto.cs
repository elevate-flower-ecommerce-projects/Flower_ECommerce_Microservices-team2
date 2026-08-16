namespace Catalog_Service.Features.Categories;

/// <summary>
/// Customer-facing category data used by the home rail and Categories screen.
/// The client owns the synthetic "All" option because it has no category ID.
/// </summary>
public sealed record CategoryDto(
    long Id,
    string Name,
    string? ImageUrl,
    int DisplayOrder);
