namespace Catalog_Service.Features.Categories.Dto;

public sealed record CategoryDto(
    long Id,
    string Name,
    string? ImageUrl,
    int DisplayOrder);
