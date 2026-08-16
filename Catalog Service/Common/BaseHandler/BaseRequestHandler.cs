using Catalog_Service.Common.Services;
using Catalog_Service.Data;
using DotNetCore.CAP;
using MediatR;

namespace Catalog_Service.Common.BaseHandler
{
    public abstract class BaseRequestHandler<TRequest, TResponse>(BaseRequestParameters baseParameters) : IRequestHandler<TRequest, TResponse>
         where TRequest : IRequest<TResponse>
    {
        protected readonly IMediator _mediator = baseParameters._mediator;
        protected readonly CatalogServiceDbContext _context = baseParameters._context;
        protected readonly IdGen.IIdGenerator<long> _snowflake = baseParameters._snowflake;
        protected readonly ICapPublisher _capPublisher = baseParameters._capPublisher;
        protected readonly IUserState _userState = baseParameters._userState;
        protected readonly CancellationTokenCapture _cancellationTokenCapture = baseParameters._cancellationTokenCapture;

        public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }
}
