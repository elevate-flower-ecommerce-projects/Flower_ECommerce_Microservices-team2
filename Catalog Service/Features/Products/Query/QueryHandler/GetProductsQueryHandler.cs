using Catalog_Service.Common.BaseHandler;
using Catalog_Service.Common.ResultPattern;
using Catalog_Service.Entities;
using Catalog_Service.Features.Products.Dto;
using Catalog_Service.Features.Products.Query;

namespace Catalog_Service.Features.Products.Query.QueryHandler;

public sealed class GetProductsQueryHandler(BaseRequestParameters baseParameters)
    : BaseRequestHandler<GetProductsQuery, RequestResult<PagedResult<ProductSummaryDto>>>(baseParameters)
{
    public override async Task<RequestResult<PagedResult<ProductSummaryDto>>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        var pagedResult = await _genericRepo.GetPagedAsync<Product, ProductSummaryDto>(
            product => new ProductSummaryDto(
                product.Id,
                product.Name,
                product.Price,
                product.DiscountPercentage,
                product.ProductStatus,
                product.Quantity,
                product.Images
                    .OrderBy(image => image.Id)
                    .Select(image => image.Url)
                    .FirstOrDefault(),
                product.CategoryId,
                product.Category.Name),
            request.PageNumber,
            request.PageSize,
            predicate: p =>
                !p.IsArchived
                && (request.CategoryId == null || p.CategoryId == request.CategoryId)
                && (request.OccasionId == null || p.ProductOccasions.Any(po => po.OccasionId == request.OccasionId)),
            cancellationToken: cancellationToken);

        return RequestResult<PagedResult<ProductSummaryDto>>.Success(pagedResult);
    }
}
