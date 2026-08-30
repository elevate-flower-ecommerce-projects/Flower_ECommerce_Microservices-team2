using Catalog_Service.Common.BaseHandler;
using Catalog_Service.Common.ResultPattern;
using Catalog_Service.Features.Catalog.Shared;
using Catalog_Service.Features.Products.GetProducts.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.Features.Products.GetProducts.Queries;

public sealed record GetProductsQuery(
    long? CategoryId = null,
    long? OccasionId = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<RequestResult<PagedResult<ProductSummaryDto>>>;

public sealed class GetProductsQueryHandler(BaseRequestParameters baseParameters)
    : BaseRequestHandler<GetProductsQuery, RequestResult<PagedResult<ProductSummaryDto>>>(baseParameters)
{
    public override async Task<RequestResult<PagedResult<ProductSummaryDto>>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        var pagedResult = await _context.Products
            .AsNoTracking()
            .Where(p =>
                !p.IsArchived
                && (request.CategoryId == null || p.CategoryId == request.CategoryId)
                && (request.OccasionId == null || p.ProductOccasions.Any(po => po.OccasionId == request.OccasionId)))
            .OrderBy(p => p.Name)
            .ThenBy(p => p.Id)
            .Select(product => new ProductSummaryDto(
                product.Id,
                product.Name,
                product.Price,
                product.DiscountPercentage,
                product.ProductStatus,
                product.Quantity,
                product.Images
                    .OrderBy(image => image.DisplayOrder)
                    .ThenBy(image => image.Id)
                    .Select(image => image.Url)
                    .FirstOrDefault(),
                product.CategoryId,
                product.Category.Name))
            .ToPagedResultAsync(request.PageNumber, request.PageSize, _cancellationTokenCapture.Token);

        return RequestResult<PagedResult<ProductSummaryDto>>.Success(pagedResult);
    }
}
