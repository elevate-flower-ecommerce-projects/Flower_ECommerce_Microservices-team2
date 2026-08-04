using MediatR;

namespace AuthService.Features.Lookup.Queries;

public class GetLookupsQuery : IRequest<GetLookupsResponse>
{
}

public class GetLookupsResponse
{
    public List<PersonTypeDto> PersonTypes { get; set; } = [];
    public List<StatusDto> Statuses { get; set; } = [];
}

public class PersonTypeDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class StatusDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
