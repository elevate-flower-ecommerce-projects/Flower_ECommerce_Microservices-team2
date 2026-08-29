namespace Cart_ServiceCart_Service.Features.Cart;

public sealed record AddCartItemRequest(long ProductId, int Quantity);

public sealed record UpdateCartItemQuantityRequest(int Quantity);

public sealed record CartItemResponse(
    long Id,
    long ProductId,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice);

public sealed record CartSummaryResponse(
    long Id,
    IReadOnlyList<CartItemResponse> Items,
    int TotalQuantity,
    decimal Subtotal,
    decimal Total);

public sealed record CartErrorResponse(string Message);
