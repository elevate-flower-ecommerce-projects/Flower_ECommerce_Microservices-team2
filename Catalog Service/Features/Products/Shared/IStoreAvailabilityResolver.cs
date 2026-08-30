using Catalog_Service.Common.Enums;

namespace Catalog_Service.Features.Products.Shared;

public interface IStoreAvailabilityResolver
{
    Task<StoreAvailabilityResolution> ResolveAsync(long productId, long? storeId, ProductStatus productStatus);
}
