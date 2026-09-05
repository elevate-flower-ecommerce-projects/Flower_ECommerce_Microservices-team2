namespace Cart_ServiceCart_Service.Features.Address;

/// <summary>
/// Response DTO for a saved delivery address.
/// </summary>
public sealed record AddressResponse(
    long Id,
    string RecipientName,
    string RecipientPhone,
    string City,
    string Area,
    string AddressLine,
    string? Label,
    decimal? Lat,
    decimal? Lng,
    bool IsDefault,
    long? StoreId = null,
    bool IsServiceable = true);

/// <summary>
/// Request payload for creating a new delivery address.
/// </summary>
public sealed record CreateAddressRequest(
    string RecipientName,
    string? Phone,
    string? RecipientPhone,
    string AddressLine,
    string City,
    string Area,
    decimal? Lat = null,
    decimal? Lng = null,
    string? Label = null,
    bool? IsDefault = null)
{
    public string EffectivePhone => !string.IsNullOrWhiteSpace(Phone) ? Phone : (RecipientPhone ?? string.Empty);
}

/// <summary>
/// Request payload for updating an existing delivery address.
/// </summary>
public sealed record UpdateAddressRequest(
    string RecipientName,
    string? Phone,
    string? RecipientPhone,
    string AddressLine,
    string City,
    string Area,
    decimal? Lat = null,
    decimal? Lng = null,
    string? Label = null,
    bool? IsDefault = null)
{
    public string EffectivePhone => !string.IsNullOrWhiteSpace(Phone) ? Phone : (RecipientPhone ?? string.Empty);
}
