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
        RequestResult<PagedResult<ProductCatalogResultDto>>>{
    public GetProductCatalogListQueryHandler(BaseRequestParameters baseParameters) : base(baseParameters) {}

    public async override Task<RequestResult<PagedResult<ProductCatalogResultDto>>> Handle(
        GetProductCatalogListQuery request,
        CancellationToken cancellationToken){
        var predicate = FilterExpression(request.categoryId, request.occasionId);

        var products = _context.Products.AsExpandable();
        var PaginatedResult = await products
            .Where(predicate)
            .OrderBy(p => p.Name)
            .ThenBy(p => p.Id)
            .Select(p => new ProductCatalogResultDto(
                p.Id,
                p.Name,
                p.Images.OrderBy(i=>i.Id).Select(i=>i.Url).FirstOrDefault(),
                p.Price,
                p.DiscountPercentage,
                ProductExpressions.DiscountedPrice.Invoke(p),
                p.Quantity
             ))
            .ToPagedResultAsync(request.PageNumber,request.PageSize, cancellationToken);

        return RequestResult<PagedResult<ProductCatalogResultDto>>
               .Success(PaginatedResult);
    }

    private static Expression<Func<Product, bool>> FilterExpression( long? categoryId, long? occasionId){
        var predicate = PredicateBuilder.New<Product>(true);
        predicate = predicate.And(p => !p.IsArchived);
        if (categoryId.HasValue) {
            predicate = predicate.And(p => p.CategoryId == categoryId.Value);
        }
        if (occasionId.HasValue){
            predicate = predicate.And(p =>
                p.ProductOccasions.Any(po => po.OccasionId == occasionId.Value));
        }
        return predicate;
    }
}
