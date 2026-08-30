using System.Linq.Expressions;
using Cart_ServiceCart_Service.Common.ResultPattern;
namespace Cart_ServiceCart_Service.Common.Interface;

/// <summary>
/// Generic repository interface providing query pagination directly from the database context.
/// </summary>
public interface IGenericRepository
{
    /// <summary>
    /// Pages the entity set for <typeparamref name="T"/> with an optional where predicate and projects to <typeparamref name="TDto"/>.
    /// </summary>
    Task<PagedResult<TDto>> GetPagedAsync<T, TDto>(
        Expression<Func<T, TDto>> selector,
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
        where T : class;

    /// <summary>
    /// Pages the entity set for <typeparamref name="T"/> with an optional where predicate directly into a <see cref="PagedResult{T}"/>.
    /// </summary>
    Task<PagedResult<T>> GetPagedAsync<T>(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
        where T : class;
}
