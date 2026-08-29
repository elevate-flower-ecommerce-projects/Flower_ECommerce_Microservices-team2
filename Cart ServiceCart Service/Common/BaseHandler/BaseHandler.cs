using Cart_ServiceCart_Service.Data;
using MediatR;

namespace Cart_ServiceCart_Service.Common.BaseHandler;

public abstract class BaseHandler<TRequest, TResponse>(BaseParameters baseParameters)
    : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    protected readonly IMediator _mediator = baseParameters._mediator;
    protected readonly CartDbContext _context = baseParameters._context;
    protected readonly IdGen.IIdGenerator<long> _snowflake = baseParameters._snowflake;

    public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
