namespace Cart_ServiceCart_Service.Entities;

/// <summary>
/// A saved delivery address for a customer.
/// </summary>
public class Address : BaseEntity
{
    public long UserId { get; set; }

    public string RecipientName { get; set; } = string.Empty;

    public string RecipientPhone { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string Area { get; set; } = string.Empty;

    public string AddressLine { get; set; } = string.Empty;

    public string? Label { get; set; }

    public decimal? Lat { get; set; }

    public decimal? Lng { get; set; }

    public bool IsDefault { get; set; } = false;

    public long? StoreId { get; set; }

    public bool IsServiceable { get; set; } = true;
}
