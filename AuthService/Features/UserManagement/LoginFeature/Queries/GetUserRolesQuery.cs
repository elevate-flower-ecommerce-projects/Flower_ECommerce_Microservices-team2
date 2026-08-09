using AuthService.Common.ResultPattern;
using MediatR;

namespace AuthService.Features.UserManagement.LoginFeature.Queries
{
    public record GetUserRolesQuery(long UserId) : IRequest<RequestResult<List<string>>>;
}
