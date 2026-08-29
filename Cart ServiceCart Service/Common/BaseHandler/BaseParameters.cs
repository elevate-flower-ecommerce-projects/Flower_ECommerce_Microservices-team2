using Cart_ServiceCart_Service.Data;
using MediatR;

namespace Cart_ServiceCart_Service.Common.BaseHandler;

public class BaseParameters(
    IMediator mediator,
    IdGen.IIdGenerator<long> idGenerator,
    CartDbContext context)
{
    public IMediator _mediator { get; } = mediator;
    public IdGen.IIdGenerator<long> _snowflake { get; } = idGenerator;
    public CartDbContext _context { get; } = context;
}
