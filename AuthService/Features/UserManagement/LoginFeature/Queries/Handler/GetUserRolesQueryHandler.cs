using AuthService.Common.BaseHandler;
using AuthService.Common.ResultPattern;
using AuthService.Features.UserManagement.LoginFeature.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AuthService.Features.UserManagement.LoginFeature.Queries.Handler
{
    public class GetUserRolesQueryHandler : BaseHandler<GetUserRolesQuery, RequestResult<List<string>>>
    {
        public GetUserRolesQueryHandler(BaseParameters baseParameters)
            : base(baseParameters)
        {
        }

        public override async Task<RequestResult<List<string>>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
        {

            var roleNames = await _context.UserRoles
                .Where(ur => ur.UserId == request.UserId)
                .Select(ur => ur.Role.Name)
                .ToListAsync(cancellationToken);

            return RequestResult<List<string>>.Success(roleNames);


        }
    }
}
