using Catalog_Service.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.Features.Categories;

public sealed class GetCategoriesQueryHandler(CatalogServiceDbContext context)
    : IRequestHandler<GetCategoriesQuery, IEnumerable<CategoryDto>>
{
    public async Task<IEnumerable<CategoryDto>> Handle(
        GetCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        // The global soft-delete filter makes this an active-only query. Reading
        // directly from the database ensures admin changes appear immediately.
        return await context.Categories
            .OrderBy(category => category.DisplayOrder)
            .ThenBy(category => category.Name)
            .ThenBy(category => category.Id)
            .Select(category => new CategoryDto(
                category.Id,
                category.Name,
                category.ImageUrl,
                category.DisplayOrder))
            .ToListAsync(cancellationToken);
    }
}
