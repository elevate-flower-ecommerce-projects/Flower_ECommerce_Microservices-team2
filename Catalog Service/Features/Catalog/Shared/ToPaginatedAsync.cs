using Catalog_Service.Common.ResultPattern;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.Features.Catalog.Shared;

public static class PaginationHelper
{
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
          this IQueryable<T> query,
          int page,
          int PageSize,
          CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;
        PageSize = PageSize < 1 ? 10 : PageSize;
        var total = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(total / (double)PageSize);
        var data = await query
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>(data, page, PageSize, total, totalPages);
    }
}
