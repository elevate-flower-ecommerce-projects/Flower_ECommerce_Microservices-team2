namespace Catalog_Service.Common.ResultPattern
{
    public record PagedResult<T>(
        List<T> Items,
        int Page,
        int PageSize,
        int TotalCount,
        int TotalPages
    );
}
