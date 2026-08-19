using Catalog_Service.Common.BaseHandler;
using Catalog_Service.Common.ResultPattern;
using Catalog_Service.Entities;
using Catalog_Service.Features.Categories.Query;
using Catalog_Service.Features.Products.Dto;

namespace Catalog_Service.Features.Categories.Query.QueryHandler;

public sealed class GetCategoryProductsQueryHandler(BaseRequestParameters baseParameters)
    : BaseRequestHandler<GetCategoryProductsQuery, RequestResult<PagedResult<ProductSummaryDto>>>(baseParameters)
{
    public override async Task<RequestResult<PagedResult<ProductSummaryDto>>> Handle(
        GetCategoryProductsQuery request,
        CancellationToken cancellationToken)
    {
        var pagedResult = await _genericRepo.GetPagedAsync<Product, ProductSummaryDto>(
            p => new ProductSummaryDto(
                p.Id,
                p.Name,
                p.Price,
                p.DiscountPercentage,
                p.ProductStatus,
                p.Quantity,
                p.Images.OrderBy(img => img.Id).Select(img => img.Url).FirstOrDefault(),
                p.CategoryId,
                p.Category.Name),
            request.PageNumber,
            request.PageSize,
            predicate: p => !p.IsArchived && p.CategoryId == request.CategoryId,
            cancellationToken: cancellationToken);

        return RequestResult<PagedResult<ProductSummaryDto>>.Success(pagedResult);
    }
}
