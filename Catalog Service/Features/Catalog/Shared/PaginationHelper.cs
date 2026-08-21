using Catalog_Service.Common.ResultPattern;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.Features.Catalog.Shared;

public static class PaginationHelper {
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
          this IQueryable<T> query,
          int page,
          int pageSize,
          CancellationToken cancellationToken = default) {

        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : pageSize;
        var total = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(total / (double)pageSize);
        var data = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>(data, page, pageSize, total, totalPages);
    }
}
