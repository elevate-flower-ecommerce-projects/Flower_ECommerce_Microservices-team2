using FluentValidation;

namespace AuthService.Features.Lookup.Queries;

public class GetLookupsQueryValidator : AbstractValidator<GetLookupsQuery>
{
    public GetLookupsQueryValidator()
    {
        // No validation needed for this query
    }
}
