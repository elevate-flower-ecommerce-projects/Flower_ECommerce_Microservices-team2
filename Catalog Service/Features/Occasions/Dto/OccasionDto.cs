namespace Catalog_Service.Features.Occasions.Dto;

public sealed record OccasionDto(
    long Id,
    string Name,
    string? ImageUrl);
