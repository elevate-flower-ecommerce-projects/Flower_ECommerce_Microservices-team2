using Catalog_Service.Common.BaseHandler;
using Catalog_Service.Common.ResultPattern;
using Catalog_Service.Entities;
using Catalog_Service.Features.Occasions.Dto;
using Catalog_Service.Features.Occasions.Query;

namespace Catalog_Service.Features.Occasions.Query.QueryHandler;

public sealed class GetOccasionProductsQueryHandler(BaseRequestParameters baseParameters)
    : BaseRequestHandler<GetOccasionProductsQuery, RequestResult<PagedResult<OccasionsProductsDto>>>(baseParameters)
{
    public override async Task<RequestResult<PagedResult<OccasionsProductsDto>>> Handle(
        GetOccasionProductsQuery request,
        CancellationToken cancellationToken)
    {
        var pagedResult = await _genericRepo.GetPagedAsync<ProductOccasion, OccasionsProductsDto>(
            po => new OccasionsProductsDto
            {
                ProductId   = po.ProductId,
                ProductName = po.Product.Name,
                Price       = po.Product.Price,
                ImageUrl    = po.Product.Images
                                  .OrderBy(img => img.Id)
                                  .Select(img => img.Url)
                                  .FirstOrDefault()
            },
            request.PageNumber,
            request.PageSize,
            predicate: po => po.OccasionId == request.OccasionId,
            cancellationToken: cancellationToken);

        return RequestResult<PagedResult<OccasionsProductsDto>>.Success(pagedResult);
    }
}
