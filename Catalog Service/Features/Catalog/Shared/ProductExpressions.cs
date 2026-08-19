using Catalog_Service.Entities;
using System.Linq.Expressions;

namespace Catalog_Service.Features.Catalog.Shared;

public static class ProductExpressions {
    public static readonly Expression<Func<Product, decimal?>> DiscountedPrice =
        p => p.DiscountPercentage.HasValue
            && (!p.DiscountStartAt.HasValue || p.DiscountStartAt <= DateTime.UtcNow)
            && (!p.DiscountEndAt.HasValue || p.DiscountEndAt >= DateTime.UtcNow)
            ? p.Price - (p.Price * p.DiscountPercentage.Value / 100)
            : (decimal?)null;
}
