using AuthService.Entities;
using MediatR;

namespace AuthService.Features.UserManagement.LoginFeature.Queries
{
    public record GetActiveRefreshTokenQuery(long UserId) : IRequest<RefreshToken?>;
}
