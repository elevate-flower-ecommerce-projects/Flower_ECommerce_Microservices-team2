using Cart_ServiceCart_Service.Common.Exceptions;
using Cart_ServiceCart_Service.Common.Interface;
using Cart_ServiceCart_Service.Data;
using MediatR;
namespace Cart_ServiceCart_Service.Common.Behaviors;

public class TransactionMiddleware<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    private readonly CartDbContext _context;

    public TransactionMiddleware(CartDbContext context)
    {
        _context = context;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // If the request explicitly opts out of transactions, continue without starting one
        if (request is INoTransaction)
            return await next();

        // If we're already inside a transaction (nested call), just continue
        if (_context.Database.CurrentTransaction != null)
            return await next();

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var response = await next();

            await _context.SaveChangesAsync();
            await transaction.CommitAsync(cancellationToken);

            return response;
        }
        catch (BusinessException)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
