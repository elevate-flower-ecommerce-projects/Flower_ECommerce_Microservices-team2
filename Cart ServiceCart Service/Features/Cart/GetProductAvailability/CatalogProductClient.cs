using System.Net;
using System.Net.Http.Json;

namespace Cart_ServiceCart_Service.Features.Cart.GetProductAvailability;

public sealed record CatalogProduct(
    long Id,
    string Name,
    decimal UnitPrice,
    int AvailableQuantity,
    bool IsActive);

public enum CatalogProductLookupStatus
{
    Found,
    NotFound,
    Error
}

public sealed record CatalogProductLookupResult(
    CatalogProductLookupStatus Status,
    CatalogProduct? Product = null);

public interface IProductCatalogClient
{
    Task<CatalogProductLookupResult> GetProductAsync(long productId, CancellationToken cancellationToken);
}

public sealed class CatalogProductClient(HttpClient httpClient) : IProductCatalogClient
{
    public async Task<CatalogProductLookupResult> GetProductAsync(long productId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await httpClient.GetAsync($"products/{productId}", cancellationToken);
            if (response.StatusCode == HttpStatusCode.NotFound)
                return new CatalogProductLookupResult(CatalogProductLookupStatus.NotFound);

            response.EnsureSuccessStatusCode();

            var product = await response.Content.ReadFromJsonAsync<CatalogProduct>(cancellationToken: cancellationToken);
            if (product is null)
                return new CatalogProductLookupResult(CatalogProductLookupStatus.Error);

            return new CatalogProductLookupResult(CatalogProductLookupStatus.Found, product);
        }
        catch
        {
            return new CatalogProductLookupResult(CatalogProductLookupStatus.Error);
        }
    }
}
