using Cart_ServiceCart_Service.Common.BaseHandler;
using Cart_ServiceCart_Service.Common.ResultPattern;
using Cart_ServiceCart_Service.Entities;
using Cart_ServiceCart_Service.Features.Cart.GetProductAvailability;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using CartEntity = Cart_ServiceCart_Service.Entities.Cart;

namespace Cart_ServiceCart_Service.Features.Cart.UpdateItemQuantity;

public sealed class UpdateItemQuantityCommandHandler(BaseParameters baseParameters)
    : BaseHandler<UpdateItemQuantityCommand, RequestResult<CartSummaryResponse>>(baseParameters)
{
    public override async Task<RequestResult<CartSummaryResponse>> Handle(
        UpdateItemQuantityCommand request,
        CancellationToken cancellationToken)
    {
        if (request.CartItemId <= 0)
            return RequestResult<CartSummaryResponse>.Failure(ErrorCode.InvalidInput, "Cart item id must be greater than zero.");

        if (request.Request.Quantity <= 0)
            return RequestResult<CartSummaryResponse>.Failure(ErrorCode.InvalidInput, "Quantity must be at least one. Remove an item instead of setting its quantity to zero.");

        var productId = await _context.CartItems
            .AsNoTracking()
            .Where(item => item.Id == request.CartItemId &&
                           item.Cart.UserId == request.UserId &&
                           !item.Cart.IsCheckOut)
            .Select(item => (long?)item.ProductId)
            .SingleOrDefaultAsync(cancellationToken);

        if (productId is null)
            return RequestResult<CartSummaryResponse>.Failure(ErrorCode.NotFound, "Cart item was not found.");

        var productResult = await _mediator.Send(new GetProductAvailabilityQuery(productId.Value), cancellationToken);
        if (!productResult.IsSuccess || productResult.Data is null)
            return RequestResult<CartSummaryResponse>.Failure(productResult.ErrorCode, productResult.Message);

        var product = productResult.Data;
        if (request.Request.Quantity > product.AvailableQuantity)
            return RequestResult<CartSummaryResponse>.Failure(ErrorCode.Conflict, $"Only {Math.Max(product.AvailableQuantity, 0)} item(s) are currently available.");

        var transaction = await BeginTransactionAsync(cancellationToken);
        try
        {
            var cart = await GetActiveCartForUpdateAsync(request.UserId, cancellationToken);
            var cartItem = cart?.CartItems.SingleOrDefault(item => item.Id == request.CartItemId);

            if (cartItem is null)
                return RequestResult<CartSummaryResponse>.Failure(ErrorCode.NotFound, "Cart item was not found.");

            cartItem.Quantity = request.Request.Quantity;
            cartItem.TotalPrice = CalculateLineTotal(product.UnitPrice, request.Request.Quantity);
            cartItem.UpdatedAt = DateTime.UtcNow;
            cartItem.UpdatedBy = request.UserId;

            RefreshCartTotal(cart!, request.UserId);
            await _context.SaveChangesAsync(cancellationToken);

            if (transaction is not null)
                await transaction.CommitAsync(cancellationToken);

            return RequestResult<CartSummaryResponse>.Success(ToSummary(cart!));
        }
        finally
        {
            if (transaction is not null)
                await transaction.DisposeAsync();
        }
    }

    private async Task<CartEntity?> GetActiveCartForUpdateAsync(long userId, CancellationToken cancellationToken)
    {
        return await _context.Carts
            .AsTracking()
            .Include(cart => cart.CartItems)
            .SingleOrDefaultAsync(cart => cart.UserId == userId && !cart.IsCheckOut, cancellationToken);
    }

    private async Task<IDbContextTransaction?> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        if (!_context.Database.IsRelational())
            return null;

        return await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
    }

    private static decimal CalculateLineTotal(decimal unitPrice, int quantity) =>
        decimal.Round(unitPrice * quantity, 2, MidpointRounding.AwayFromZero);

    private static void RefreshCartTotal(CartEntity cart, long userId)
    {
        cart.TotalPrice = decimal.Round(cart.CartItems.Sum(item => item.TotalPrice), 2, MidpointRounding.AwayFromZero);
        cart.UpdatedAt = DateTime.UtcNow;
        cart.UpdatedBy = userId;
    }

    private static CartSummaryResponse ToSummary(CartEntity cart)
    {
        var items = cart.CartItems
            .OrderBy(item => item.CreatedAt)
            .Select(item => new CartItemResponse(
                item.Id,
                item.ProductId,
                item.Quantity,
                decimal.Round(item.TotalPrice / item.Quantity, 2),
                item.TotalPrice))
            .ToList();

        return new CartSummaryResponse(
            cart.Id,
            items,
            items.Sum(item => item.Quantity),
            cart.TotalPrice,
            cart.TotalPrice);
    }
}
