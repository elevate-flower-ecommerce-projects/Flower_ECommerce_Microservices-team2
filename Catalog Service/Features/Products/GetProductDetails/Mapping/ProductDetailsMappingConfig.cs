using Catalog_Service.Entities;
using Catalog_Service.Features.Occasions.Dto;
using Catalog_Service.Features.Products.GetProductDetails.Dto;
using Catalog_Service.Features.Products.Shared;
using Mapster;

namespace Catalog_Service.Features.Products.GetProductDetails.Mapping;

public sealed class ProductDetailsMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {

        config.NewConfig<ProductImage, ProductGalleryImageDto>()
              .MapToConstructor(true);

        config.NewConfig<ProductInclude, ProductIncludeDto>()
              .MapToConstructor(true);

        config.NewConfig<Occasion, OccasionDto>()
              .MapToConstructor(true);

        config.NewConfig<Product, ProductDetailsProjection>()
              .Map(dest => dest.Status, src => src.ProductStatus)
              .Map(dest => dest.CategoryName, src => src.Category.Name)
              .Map(dest => dest.Images, src => src.Images.OrderBy(i => i.DisplayOrder).ThenBy(i => i.Id))
              .Map(dest => dest.Includes, src => src.Includes.OrderBy(i => i.DisplayOrder).ThenBy(i => i.Id))
              .Map(dest => dest.Occasions, src => src.ProductOccasions.Select(po => po.Occasion).OrderBy(o => o.Name));

        config.NewConfig<StoreAvailabilityResolution, ProductAvailabilityDto>()
              .MapToConstructor(true);
    }
}
