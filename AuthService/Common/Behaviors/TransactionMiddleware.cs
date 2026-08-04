using AuthService.Common.Exceptions;
using AuthService.Common.Interface;
using AuthService.Data;
using MediatR;

namespace AuthService.Common.Behaviors
{
    public class TransactionMiddleware<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICommand<TResponse>
    {
        private readonly AuthDbContext _context;

        public TransactionMiddleware(AuthDbContext context)
        {
            _context = context;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
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
}
