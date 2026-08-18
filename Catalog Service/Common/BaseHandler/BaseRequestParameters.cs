using Catalog_Service.Common.Interface;
using Catalog_Service.Common.Services;
using Catalog_Service.Data;
using DotNetCore.CAP;
using MediatR;

namespace Catalog_Service.Common.BaseHandler;

public class BaseRequestParameters(
    IMediator mediator,
    IdGen.IIdGenerator<long> idGenerator,
    ICapPublisher capPublisher,
    CatalogServiceDbContext context,
    IUserState userState,
    CancellationTokenCapture cancellationTokenCapture,
    IGenericRepository genericRepository)
{
    public IMediator _mediator { get; } = mediator;
    public IdGen.IIdGenerator<long> _snowflake { get; } = idGenerator;
    public ICapPublisher _capPublisher { get; } = capPublisher;
    public CatalogServiceDbContext _context { get; } = context;
    public IUserState _userState { get; } = userState;
    public CancellationTokenCapture _cancellationTokenCapture { get; } = cancellationTokenCapture;
    public IGenericRepository _genericRepository { get; } = genericRepository;
}
