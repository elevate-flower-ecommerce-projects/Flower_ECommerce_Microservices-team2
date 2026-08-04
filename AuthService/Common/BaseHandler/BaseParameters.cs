using AuthService.Common.Services;
using AuthService.Data;
using DotNetCore.CAP;
using MediatR;

namespace AuthService.Common.BaseHandler;

public class BaseParameters(
    IMediator mediator,
    IdGen.IIdGenerator<long> idGenerator,
    ICapPublisher capPublisher,
    AuthDbContext context,
    CurrentUserService currentUserService)
{
    public IMediator _mediator { get; } = mediator;
    public IdGen.IIdGenerator<long> _snowflake { get; } = idGenerator;
    public ICapPublisher _capPublisher { get; } = capPublisher;
    public AuthDbContext _context { get; } = context;
    public CurrentUserService _currentUserService { get; } = currentUserService;
}
