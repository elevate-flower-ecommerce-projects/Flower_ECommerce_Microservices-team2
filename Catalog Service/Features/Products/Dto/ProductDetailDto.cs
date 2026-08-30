namespace Catalog_Service.Features.Products.Dto;

public sealed record ProductDetailDto(
    long Id,
    string Name,
    decimal UnitPrice,
    int AvailableQuantity,
    bool IsActive,
    decimal Price,
    int Quantity,
    string? Description,
    long CategoryId,
    string? CategoryName);
