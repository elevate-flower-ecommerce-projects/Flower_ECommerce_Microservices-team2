using Catalog_Service.Common.Enums;

namespace Catalog_Service.Features.Sections.Dto;

public sealed record SectionDto(
    long Id,
    string Title,
    int Order,
    SectionType Type,
    bool IsEnabled,
    string? ContentRefJson);
