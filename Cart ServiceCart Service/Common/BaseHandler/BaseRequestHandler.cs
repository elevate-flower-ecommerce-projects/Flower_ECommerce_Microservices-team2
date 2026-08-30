using Cart_ServiceCart_Service.Common.Interface;
using Cart_ServiceCart_Service.Common.Services;
using Cart_ServiceCart_Service.Data;
using DotNetCore.CAP;
using MediatR;
namespace Cart_ServiceCart_Service.Common.BaseHandler;

public abstract class BaseRequestHandler<TRequest, TResponse>(BaseRequestParameters baseParameters)
     : IRequestHandler<TRequest, TResponse>
     where TRequest : IRequest<TResponse>
{
    protected readonly IMediator _mediator                         = baseParameters._mediator;
    protected readonly CartDbContext _context            = baseParameters._context;
    protected readonly IdGen.IIdGenerator<long> _snowflake         = baseParameters._snowflake;
    protected readonly ICapPublisher _capPublisher                  = baseParameters._capPublisher;
    protected readonly IUserState _userState                        = baseParameters._userState;
    protected readonly CancellationTokenCapture _cancellationTokenCapture = baseParameters._cancellationTokenCapture;

    /// <summary>
    /// Generic repository helper — provides <see cref="IQueryable{T}"/> table queries
    /// and pagination methods returning <see cref="ResultPattern.PagedResult{T}"/>.
    /// </summary>
    protected readonly IGenericRepository _genericRepo             = baseParameters._genericRepository;

    public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
