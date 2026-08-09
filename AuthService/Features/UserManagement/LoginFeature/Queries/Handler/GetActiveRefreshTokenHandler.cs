using AuthService.Common.BaseHandler;
using AuthService.Entities;
using AuthService.Features.UserManagement.LoginFeature.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AuthService.Features.UserManagement.LoginFeature.Queries.Handler
{
    public class GetActiveRefreshTokenHandler : BaseHandler<GetActiveRefreshTokenQuery, RefreshToken?>
    {
        public GetActiveRefreshTokenHandler(BaseParameters baseParameters, ILogger<GetActiveRefreshTokenHandler> logger)
            : base(baseParameters)
        {
        }

        public override async Task<RefreshToken?> Handle(GetActiveRefreshTokenQuery request, CancellationToken cancellationToken)
        {
                var activeToken = await _context.RefreshTokens
                    .FirstOrDefaultAsync(rt => rt.UserId == request.UserId && !rt.IsDeleted && rt.ExpireDate > DateTime.UtcNow, cancellationToken);

                return activeToken;
         
        }
    }
}
