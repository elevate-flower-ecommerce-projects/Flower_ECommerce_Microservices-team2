using Cart_ServiceCart_Service.Common.BaseHandler;
using Cart_ServiceCart_Service.Common.ResultPattern;
using Cart_ServiceCart_Service.Entities;
using Cart_ServiceCart_Service.Features.Cart.GetProductAvailability;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using CartEntity = Cart_ServiceCart_Service.Entities.Cart;

namespace Cart_ServiceCart_Service.Features.Cart.AddItem.NewFolder;

public sealed class AddItemCommandHandler(BaseParameters baseParameters)
    : BaseHandler<AddItemCommand, RequestResult<CartSummaryResponse>>(baseParameters)
{
    public override async Task<RequestResult<CartSummaryResponse>> Handle(
        AddItemCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Request.ProductId <= 0)
            return RequestResult<CartSummaryResponse>.Failure(ErrorCode.InvalidInput, "Product id must be greater than zero.");

        if (request.Request.Quantity <= 0)
            return RequestResult<CartSummaryResponse>.Failure(ErrorCode.InvalidInput, "Quantity must be at least one. Remove an item instead of setting its quantity to zero.");

        var productResult = await _mediator.Send(new GetProductAvailabilityQuery(request.Request.ProductId), cancellationToken);
        if (!productResult.IsSuccess || productResult.Data is null)
            return RequestResult<CartSummaryResponse>.Failure(productResult.ErrorCode, productResult.Message);

        var product = productResult.Data;
        var transaction = await BeginTransactionAsync(cancellationToken);

        try
        {
            var cart = await GetActiveCartForUpdateAsync(request.UserId, cancellationToken);
            if (cart is null)
            {
                cart = new CartEntity
                {
                    Id = _snowflake.CreateId(),
                    UserId = request.UserId,
                    CreatedBy = request.UserId
                };

                _context.Carts.Add(cart);
            }

            var existingItem = cart.CartItems.SingleOrDefault(item => item.ProductId == request.Request.ProductId);
            var requestedQuantity = existingItem is null
                ? request.Request.Quantity
                : AddQuantities(existingItem.Quantity, request.Request.Quantity);

            if (requestedQuantity > product.AvailableQuantity)
                return RequestResult<CartSummaryResponse>.Failure(ErrorCode.Conflict, $"Only {Math.Max(product.AvailableQuantity, 0)} item(s) are currently available.");

            if (existingItem is null)
            {
                cart.CartItems.Add(new CartItem
                {
                    Id = _snowflake.CreateId(),
                    ProductId = product.Id,
                    Quantity = requestedQuantity,
                    TotalPrice = CalculateLineTotal(product.UnitPrice, requestedQuantity),
                    CreatedBy = request.UserId
                });
            }
            else
            {
                existingItem.Quantity = requestedQuantity;
                existingItem.TotalPrice = CalculateLineTotal(product.UnitPrice, requestedQuantity);
                existingItem.UpdatedAt = DateTime.UtcNow;
                existingItem.UpdatedBy = request.UserId;
            }

            RefreshCartTotal(cart, request.UserId);
            await _context.SaveChangesAsync(cancellationToken);

            if (transaction is not null)
                await transaction.CommitAsync(cancellationToken);

            return RequestResult<CartSummaryResponse>.Success(ToSummary(cart));
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

    private static int AddQuantities(int currentQuantity, int quantityToAdd)
    {
        try
        {
            return checked(currentQuantity + quantityToAdd);
        }
        catch (OverflowException)
        {
            throw new InvalidOperationException("Requested quantity is too large.");
        }
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
