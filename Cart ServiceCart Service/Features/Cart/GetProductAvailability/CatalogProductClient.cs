using System.Net;
using System.Text.Json;

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

            if (!response.IsSuccessStatusCode)
                return new CatalogProductLookupResult(CatalogProductLookupStatus.Error);

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var productElement = root;
            if (root.TryGetProperty("data", out var dataElement) && dataElement.ValueKind == JsonValueKind.Object)
            {
                productElement = dataElement;
            }

            var id = productElement.TryGetProperty("id", out var idProp) ? idProp.GetInt64() : productId;
            var name = productElement.TryGetProperty("name", out var nameProp) ? nameProp.GetString() ?? string.Empty : string.Empty;
            
            decimal unitPrice = 0m;
            if (productElement.TryGetProperty("unitPrice", out var upProp))
                unitPrice = upProp.GetDecimal();
            else if (productElement.TryGetProperty("price", out var pProp))
                unitPrice = pProp.GetDecimal();

            int availableQty = 0;
            if (productElement.TryGetProperty("availableQuantity", out var aqProp))
                availableQty = aqProp.GetInt32();
            else if (productElement.TryGetProperty("quantity", out var qProp))
                availableQty = qProp.GetInt32();
            else if (productElement.TryGetProperty("stockQuantity", out var sqProp))
                availableQty = sqProp.GetInt32();

            bool isActive = true;
            if (productElement.TryGetProperty("isActive", out var iaProp))
                isActive = iaProp.GetBoolean();

            var product = new CatalogProduct(id, name, unitPrice, availableQty, isActive);
            return new CatalogProductLookupResult(CatalogProductLookupStatus.Found, product);
        }
        catch
        {
            return new CatalogProductLookupResult(CatalogProductLookupStatus.Error);
        }
    }
}
