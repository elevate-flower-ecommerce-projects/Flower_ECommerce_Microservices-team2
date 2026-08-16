using Catalog_Service.Common.BaseHandler;
using Catalog_Service.Common.Enums;
using Catalog_Service.Common.ResultPattern;
using Catalog_Service.Entities;
using Catalog_Service.Features.Catalog.GetProductCatalogeList.Query;
using Catalog_Service.Features.Catalog.Shared;
using LinqKit;
using System.Linq.Expressions;

namespace Catalog_Service.Features.Catalog.GetProductCatalogeList.Handler;

public class GetProductCatalogListQueryHandler
    : BaseRequestHandler<GetProductCatalogListQuery,
        RequestResult<PagedResult<ProductCatalogResultDto>>>
{
    public GetProductCatalogListQueryHandler(BaseRequestParameters baseParameters) : base(baseParameters)
    {
    }

    public async override Task<RequestResult<PagedResult<ProductCatalogResultDto>>> Handle(
        GetProductCatalogListQuery request,
        CancellationToken cancellationToken)
    {
        var predicate = BuildFilterExpression(request.categoryId, request.occasionId);

        var products = _context.Products;
        var PaginatedResult = await products.Where(predicate)
            .Select(p => new ProductCatalogResultDto(
                p.Id,
                p.Name,
                p.Images.Select(i=>i.Url).FirstOrDefault(),
                p.Price,
                p.DiscountPercentage,
                p.Price-p.DiscountPercentage,
                p.Quantity
             ))
            .ToPagedResultAsync(request.PageNumber,request.PageSize);

        return RequestResult<PagedResult<ProductCatalogResultDto>>
               .Success(PaginatedResult);
    }

    private static Expression<Func<Product, bool>> BuildFilterExpression( long? categoryId, long? occasionId)
    {
        var predicate = PredicateBuilder.New<Product>(true);
        if (categoryId.HasValue)
        {
            predicate = predicate.And(p => p.CategoryId == categoryId.Value);
        }
        if (occasionId.HasValue)
        {
            predicate = predicate.And(p =>
                p.ProductOccasions.Any(po => po.OccasionId == occasionId.Value));
        }
        return predicate;
    }

   
}
