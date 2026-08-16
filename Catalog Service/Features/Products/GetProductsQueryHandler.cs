using Catalog_Service.Common.Enums;
using Catalog_Service.Common.ResultPattern;
using Catalog_Service.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.Features.Products;

public sealed class GetProductsQueryHandler(CatalogServiceDbContext context)
    : IRequestHandler<GetProductsQuery, RequestResult<IEnumerable<ProductSummaryDto>>>
{
    public async Task<RequestResult<IEnumerable<ProductSummaryDto>>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        if (request.CategoryId is not null)
        {
            var category = await context.Categories
                .IgnoreQueryFilters()
                .Where(category => category.Id == request.CategoryId.Value)
                .Select(category => new { category.IsDeleted })
                .SingleOrDefaultAsync(cancellationToken);

            if (category is null)
            {
                return RequestResult<IEnumerable<ProductSummaryDto>>.Failure(
                    ErrorCode.CategoryNotFound);
            }

            if (category.IsDeleted)
            {
                return RequestResult<IEnumerable<ProductSummaryDto>>.Failure(
                    ErrorCode.CategoryNoLongerAvailable);
            }
        }

        var query = context.Products
            .Where(product => !product.IsArchived);

        if (request.CategoryId is not null)
        {
            query = query.Where(product => product.CategoryId == request.CategoryId.Value);
        }

        var products = await query
            .OrderBy(product => product.Name)
            .ThenBy(product => product.Id)
            .Select(product => new ProductSummaryDto(
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
                product.Category.Name))
            .ToListAsync(cancellationToken);

        return RequestResult<IEnumerable<ProductSummaryDto>>.Success(products);
    }
}
