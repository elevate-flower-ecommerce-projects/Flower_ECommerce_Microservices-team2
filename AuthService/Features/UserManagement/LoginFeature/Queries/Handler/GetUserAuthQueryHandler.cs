using AuthService.Common.BaseHandler;
using AuthService.Common.ResultPattern;
using AuthService.Features.UserManagement.LoginFeature.Dto;
using AuthService.Features.UserManagement.LoginFeature.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AuthService.Features.UserManagement.LoginFeature.Queries.Handler
{
    public class GetUserAuthQueryHandler : BaseHandler<GetUserAuthQuery, RequestResult<UserAuthDto>>
    {
        public GetUserAuthQueryHandler(BaseParameters baseParameters) : base(baseParameters) { }

        public override async Task<RequestResult<UserAuthDto>> Handle(GetUserAuthQuery request, CancellationToken cancellationToken)
        {
            var userAuth = await _context.Users
                .Where(u => u.Email == request.Email)
                .Select(u => new UserAuthDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Gender = u.Gender,
                    Password = u.Password
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (userAuth == null)
                return RequestResult<UserAuthDto>.Failure(AuthService.Common.Enums.ErrorCode.UserNotFound, "User not found.");

            return RequestResult<UserAuthDto>.Success(userAuth);
        }
    }
}
