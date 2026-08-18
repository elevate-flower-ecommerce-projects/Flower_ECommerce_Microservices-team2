using Catalog_Service.Common.BaseHandler;
using Catalog_Service.Common.ResultPattern;
using Catalog_Service.Features.Categories.Dto;
using Catalog_Service.Features.Categories.Query;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.Features.Categories.Query.QueryHandler;

public sealed class GetCategoriesQueryHandler(BaseRequestParameters baseParameters)
    : BaseRequestHandler<GetCategoriesQuery, RequestResult<IEnumerable<CategoryDto>>>(baseParameters)
{
    public override async Task<RequestResult<IEnumerable<CategoryDto>>> Handle(
        GetCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var categories = await _context.Categories
            .OrderBy(category => category.DisplayOrder)
            .Select(category => new CategoryDto(
                category.Id,
                category.Name,
                category.ImageUrl,
                category.DisplayOrder))
            .ToListAsync(cancellationToken);

        return RequestResult<IEnumerable<CategoryDto>>.Success(categories);
    }
}
