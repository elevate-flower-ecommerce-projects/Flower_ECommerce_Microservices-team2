using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Cart_ServiceCart_Service.Features.Address.Services;

/// <summary>
/// Default implementation of <see cref="IGeoLookupService"/> that resolves an address
/// to a serving store by querying the Address/Store coverage service or matching known coverage zones.
/// </summary>
public sealed class GeoLookupService : IGeoLookupService
{
    private readonly HttpClient? _httpClient;
    private readonly ILogger<GeoLookupService> _logger;

    // Known active store mappings (mirroring Catalog / Store Coverage database)
    // Store 6001: "Flowery Zamalek"    -> "Cairo - Zamalek" (Active)
    // Store 6002: "Flowery Nasr City"  -> "Cairo - Nasr City" (Active)
    // Store 6003: "Flowery Alexandria" -> "Alexandria - Downtown" (Active)
    // Store 6004: "Flowery Dokki"      -> "Giza - Dokki" (Inactive / unserviceable)

    private const long ZamalekStoreId = 6001;
    private const long NasrCityStoreId = 6002;
    private const long AlexandriaStoreId = 6003;

    public GeoLookupService(ILogger<GeoLookupService> logger, HttpClient? httpClient = null)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<long?> ResolveServingStoreIdAsync(
        string city,
        string area,
        decimal? lat = null,
        decimal? lng = null,
        CancellationToken cancellationToken = default)
    {
        // 1. If an HTTP client is configured with a BaseAddress, attempt remote lookup first
        if (_httpClient?.BaseAddress is not null)
        {
            try
            {
                var queryParams = $"city={Uri.EscapeDataString(city ?? string.Empty)}&area={Uri.EscapeDataString(area ?? string.Empty)}";
                if (lat.HasValue && lng.HasValue)
                {
                    queryParams += $"&lat={lat.Value}&lng={lng.Value}";
                }

                var response = await _httpClient.GetAsync($"stores/resolve?{queryParams}", cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync(cancellationToken);
                    using var doc = JsonDocument.Parse(content);
                    var root = doc.RootElement;

                    if (root.TryGetProperty("storeId", out var storeIdProp) && storeIdProp.ValueKind == JsonValueKind.Number)
                    {
                        var resolvedId = storeIdProp.GetInt64();
                        _logger.LogInformation("Remote geo lookup resolved address ({City}, {Area}) to store {StoreId}", city, area, resolvedId);
                        return resolvedId;
                    }
                    if (root.TryGetProperty("data", out var dataProp) && dataProp.ValueKind == JsonValueKind.Object &&
                        dataProp.TryGetProperty("storeId", out var innerStoreId) && innerStoreId.ValueKind == JsonValueKind.Number)
                    {
                        var resolvedId = innerStoreId.GetInt64();
                        _logger.LogInformation("Remote geo lookup resolved address ({City}, {Area}) to store {StoreId}", city, area, resolvedId);
                        return resolvedId;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Remote geo lookup failed for ({City}, {Area}), falling back to local coverage resolver.", city, area);
            }
        }

        // 2. Fallback to local coverage zone resolution
        return ResolveLocally(city, area, lat, lng);
    }

    private long? ResolveLocally(string? city, string? area, decimal? lat, decimal? lng)
    {
        var normalizedCity = (city ?? string.Empty).Trim().ToLowerInvariant();
        var normalizedArea = (area ?? string.Empty).Trim().ToLowerInvariant();

        // Check Zamalek (Store 6001)
        if (normalizedArea.Contains("zamalek") || normalizedArea.Contains("الزمالك"))
        {
            return ZamalekStoreId;
        }

        // Check Nasr City (Store 6002)
        if (normalizedArea.Contains("nasr city") || normalizedArea.Contains("madinet nasr") || normalizedArea.Contains("مدينة نصر"))
        {
            return NasrCityStoreId;
        }

        // Check Alexandria Downtown (Store 6003)
        if (normalizedCity.Contains("alex") || normalizedCity.Contains("إسكندرية") || normalizedCity.Contains("اسكندرية"))
        {
            if (normalizedArea.Contains("downtown") || normalizedArea.Contains("وسط البلد") ||
                normalizedArea.Contains("raml") || normalizedArea.Contains("mansheya") ||
                normalizedArea.Contains("alex") || string.IsNullOrWhiteSpace(normalizedArea))
            {
                return AlexandriaStoreId;
            }
        }

        // Check Dokki (Store 6004 - Inactive, unserviceable)
        if (normalizedArea.Contains("dokki") || normalizedArea.Contains("الدقي"))
        {
            _logger.LogInformation("Store 6004 covering Dokki is currently inactive. Address flagged as unserviceable.");
            return null;
        }

        // Coordinate-based approximate resolution (if coordinates were passed)
        if (lat.HasValue && lng.HasValue)
        {
            var latitude = (double)lat.Value;
            var longitude = (double)lng.Value;

            // Approximate bounding box for Zamalek: Lat [30.045, 30.080], Lng [31.210, 31.235]
            if (latitude >= 30.045 && latitude <= 30.080 && longitude >= 31.210 && longitude <= 31.235)
                return ZamalekStoreId;

            // Approximate bounding box for Nasr City: Lat [30.030, 30.090], Lng [31.310, 31.380]
            if (latitude >= 30.030 && latitude <= 30.090 && longitude >= 31.310 && longitude <= 31.380)
                return NasrCityStoreId;

            // Approximate bounding box for Alexandria Downtown: Lat [31.180, 31.220], Lng [29.890, 29.930]
            if (latitude >= 31.180 && latitude <= 31.220 && longitude >= 29.890 && longitude <= 29.930)
                return AlexandriaStoreId;
        }

        // If City is Cairo and area contains "cairo", default to nearest central Cairo store (Zamalek)
        if (normalizedCity == "cairo" && (normalizedArea == "cairo" || normalizedArea == "downtown" || normalizedArea == "وسط البلد"))
        {
            return ZamalekStoreId;
        }

        _logger.LogInformation("Address ({City}, {Area}) could not be resolved to any active store. Flagged as unserviceable.", city, area);
        return null;
    }
}
