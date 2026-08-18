namespace Catalog_Service.Features.Occasions.Dto;

public class OccasionsProductsDto
{
    public long ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
}
