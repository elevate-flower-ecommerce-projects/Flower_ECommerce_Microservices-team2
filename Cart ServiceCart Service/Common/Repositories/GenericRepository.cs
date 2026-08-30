using System.Linq.Expressions;
using Cart_ServiceCart_Service.Common.Interface;
using Cart_ServiceCart_Service.Common.ResultPattern;
using Cart_ServiceCart_Service.Data;
using Microsoft.EntityFrameworkCore;
namespace Cart_ServiceCart_Service.Common.Repositories;

/// <summary>
/// Implementation of <see cref="IGenericRepository"/> with injected <see cref="CatalogServiceDbContext"/>.
/// </summary>
public class GenericRepository(CartDbContext context) : IGenericRepository
{
    private readonly CartDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

    /// <inheritdoc />
    public async Task<PagedResult<TDto>> GetPagedAsync<T, TDto>(
        Expression<Func<T, TDto>> selector,
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
        where T : class
    {
        pageNumber = Math.Max(1, pageNumber);
        pageSize   = Math.Max(1, pageSize);

        var query = _context.Set<T>().AsNoTracking();

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(selector)
            .ToListAsync(cancellationToken);

        return new PagedResult<TDto>(items, pageNumber, pageSize, totalCount, totalPages);
    }

    /// <inheritdoc />
    public async Task<PagedResult<T>> GetPagedAsync<T>(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
        where T : class
    {
        pageNumber = Math.Max(1, pageNumber);
        pageSize   = Math.Max(1, pageSize);

        var query = _context.Set<T>().AsNoTracking();

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>(items, pageNumber, pageSize, totalCount, totalPages);
    }
}
