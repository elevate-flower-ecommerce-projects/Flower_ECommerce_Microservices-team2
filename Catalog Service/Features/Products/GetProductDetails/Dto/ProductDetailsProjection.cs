using Catalog_Service.Common.Enums;
using Catalog_Service.Features.Occasions.Dto;

namespace Catalog_Service.Features.Products.GetProductDetails.Dto;

internal sealed class ProductDetailsProjection
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ProductStatus Status { get; set; }

    public bool IsArchived { get; set; }

    public decimal Price { get; set; }

    public decimal? DiscountPercentage { get; set; }

    public DateTime? DiscountStartAt { get; set; }

    public DateTime? DiscountEndAt { get; set; }

    public long CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public List<ProductGalleryImageDto> Images { get; set; } = [];

    public List<ProductIncludeDto> Includes { get; set; } = [];

    public List<OccasionDto> Occasions { get; set; } = [];
}
