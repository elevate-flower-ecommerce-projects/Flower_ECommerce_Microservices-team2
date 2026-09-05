namespace Cart_ServiceCart_Service.Features.Address.Services;

/// <summary>
/// Service responsible for resolving an address to a serving store / coverage area.
/// </summary>
public interface IGeoLookupService
{
    /// <summary>
    /// Resolves the serving store ID for the given delivery address details.
    /// Returns the active store ID if the address is within a covered zone, or null if unresolved/unserviceable.
    /// </summary>
    Task<long?> ResolveServingStoreIdAsync(
        string city,
        string area,
        decimal? lat = null,
        decimal? lng = null,
        CancellationToken cancellationToken = default);
}
