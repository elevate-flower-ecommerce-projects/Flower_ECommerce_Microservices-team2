using System.ComponentModel;

namespace Catalog_Service.Common.Enums;

/// <summary>
/// Outcome of resolving a product's availability against a customer's store.
/// Only <see cref="InStock"/> allows the client to enable Add to Cart.
/// </summary>
public enum AvailabilityStatus
{
    /// <summary>No storeId was supplied — the client must prompt for a delivery address first.</summary>
    [Description("Set a delivery address to see availability for your area.")]
    StoreNotSelected = 1,

    /// <summary>A storeId was supplied but it does not resolve to an active store.</summary>
    [Description("This product is currently unavailable for the selected area.")]
    StoreUnavailable = 2,

    /// <summary>The store resolved, but it does not carry this product at all.</summary>
    [Description("Out of stock")]
    NotCarried = 3,

    /// <summary>The store carries the product but has no units on hand.</summary>
    [Description("Out of stock")]
    OutOfStock = 4,

    /// <summary>The product is no longer sold anywhere in the catalog.</summary>
    [Description("This product has been discontinued.")]
    Discontinued = 5,

    /// <summary>Stock is available at the resolved store and the product can be added to the cart.</summary>
    [Description("In stock")]
    InStock = 6
}
