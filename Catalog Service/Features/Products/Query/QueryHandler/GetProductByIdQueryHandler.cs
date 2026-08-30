using Catalog_Service.Common.BaseHandler;
using Catalog_Service.Common.Enums;
using Catalog_Service.Common.ResultPattern;
using Catalog_Service.Entities;
using Catalog_Service.Features.Products.Dto;
using Catalog_Service.Features.Products.Query;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.Features.Products.Query.QueryHandler;

public sealed class GetProductByIdQueryHandler(BaseRequestParameters baseParameters)
    : BaseRequestHandler<GetProductByIdQuery, RequestResult<ProductDetailDto>>(baseParameters)
{
    public override async Task<RequestResult<ProductDetailDto>> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

        if (product is null || product.IsArchived)
        {
            return RequestResult<ProductDetailDto>.Failure(
                ErrorCode.ProductNotFound,
                "Product was not found.");
        }

        var isActive = product.ProductStatus == ProductStatus.Available && !product.IsArchived;
        var dto = new ProductDetailDto(
            product.Id,
            product.Name,
            product.Price,
            product.Quantity,
            isActive,
            product.Price,
            product.Quantity,
            product.Description,
            product.CategoryId,
            product.Category?.Name);

        return RequestResult<ProductDetailDto>.Success(dto);
    }
}
