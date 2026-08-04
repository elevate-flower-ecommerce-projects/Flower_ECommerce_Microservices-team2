using AuthService.Data;
using AuthService.Common.BaseHandler;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace AuthService.Features.Lookup.Queries;

public class GetLookupsQueryHandler : BaseHandler<GetLookupsQuery, GetLookupsResponse>
{
    public GetLookupsQueryHandler(BaseParameters baseParameters) : base(baseParameters)
    {
    }

    public override async Task<GetLookupsResponse> Handle(GetLookupsQuery request, CancellationToken cancellationToken)
    {
        var personTypes = await _context.PersonTypes
            .AsTracking()  // Allow tracking for this read operation if needed
            .Select(p => new PersonTypeDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description
            })
            .ToListAsync(cancellationToken);

        var statuses = await _context.Statuses
            .AsTracking()
            .Select(s => new StatusDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description
            })
            .ToListAsync(cancellationToken);

        return new GetLookupsResponse
        {
            PersonTypes = personTypes,
            Statuses = statuses
        };
    }
}
