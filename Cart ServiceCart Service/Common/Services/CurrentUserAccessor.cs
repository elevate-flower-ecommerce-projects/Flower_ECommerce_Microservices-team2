using System.Security.Claims;
using Cart_ServiceCart_Service.Common.Interfaces;

namespace Cart_ServiceCart_Service.Common.Services;

public sealed class CurrentUserAccessor(IHttpContextAccessor httpContextAccessor) : ICurrentUserAccessor
{
    public long? UserId
    {
        get
        {
            var principal = httpContextAccessor.HttpContext?.User;
            var value = principal?.FindFirstValue(ClaimTypes.NameIdentifier);

            return long.TryParse(value, out var userId) && userId > 0
                ? userId
                : null;
        }
    }
}
