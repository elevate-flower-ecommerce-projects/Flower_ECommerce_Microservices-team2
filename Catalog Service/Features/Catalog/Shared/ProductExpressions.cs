using Catalog_Service.Entities;
using System.Linq.Expressions;
using LinqKit;

namespace Catalog_Service.Features.Catalog.Shared;

public static class ProductExpressions {
    /// <summary>
    /// Computes the discounted price at query time based on active discounts.
    /// Consistent with Product Details pricing calculation.
    /// </summary>
    public static readonly Expression<Func<Product, decimal?>> DiscountedPrice =
        p => p.DiscountPercentage.HasValue
            && (!p.DiscountStartAt.HasValue || p.DiscountStartAt <= DateTime.UtcNow)
            && (!p.DiscountEndAt.HasValue || p.DiscountEndAt >= DateTime.UtcNow)
            ? Math.Round(p.Price * (1 - (p.DiscountPercentage.Value / 100m)), 2)
            : (decimal?)null;
}
