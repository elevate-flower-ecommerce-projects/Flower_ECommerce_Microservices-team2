using AuthService.Common.Enums;
using AuthService.Common.ResultPattern;
using MediatR;

namespace AuthService.Features.Common.Role.Query
{
    public record GetRoleByEnumQuery(PersonTypeEnum PersonType) : IRequest<RequestResult<long>>;
}
