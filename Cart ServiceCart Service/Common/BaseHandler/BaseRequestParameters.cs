using Cart_ServiceCart_Service.Common.Interface;
using Cart_ServiceCart_Service.Common.Services;
using Cart_ServiceCart_Service.Data;
using DotNetCore.CAP;
using MediatR;
namespace Cart_ServiceCart_Service.Common.BaseHandler;

public class BaseRequestParameters(
    IMediator mediator,
    IdGen.IIdGenerator<long> idGenerator,
    ICapPublisher capPublisher,
    CartDbContext context,
    IUserState userState,
    CancellationTokenCapture cancellationTokenCapture,
    IGenericRepository genericRepository)
{
    public IMediator _mediator { get; } = mediator;
    public IdGen.IIdGenerator<long> _snowflake { get; } = idGenerator;
    public ICapPublisher _capPublisher { get; } = capPublisher;
    public CartDbContext _context { get; } = context;
    public IUserState _userState { get; } = userState;
    public CancellationTokenCapture _cancellationTokenCapture { get; } = cancellationTokenCapture;
    public IGenericRepository _genericRepository { get; } = genericRepository;
}
