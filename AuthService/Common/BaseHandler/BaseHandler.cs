using AuthService.Common.Services;
using AuthService.Data;
using DotNetCore.CAP;
using MediatR;

namespace AuthService.Common.BaseHandler
{
    public abstract class BaseHandler<TRequest, TResponse>(BaseParameters baseParameters) : IRequestHandler<TRequest, TResponse>
         where TRequest : IRequest<TResponse>
    {
        protected readonly IMediator _mediator = baseParameters._mediator;
        protected readonly AuthDbContext _context = baseParameters._context;
        protected readonly IdGen.IIdGenerator<long> _snowflake = baseParameters._snowflake;
        protected readonly ICapPublisher _capPublisher = baseParameters._capPublisher;
        protected readonly CurrentUserService _currentUserService = baseParameters._currentUserService;

        public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }
}
