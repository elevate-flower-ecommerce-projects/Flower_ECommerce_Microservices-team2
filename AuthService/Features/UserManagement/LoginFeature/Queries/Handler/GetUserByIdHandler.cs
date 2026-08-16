using AuthService.Common.BaseHandler;
using AuthService.Common.ResultPattern;
using AuthService.Features.UserManagement.LoginFeature.Dto;
using AuthService.Features.UserManagement.LoginFeature.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AuthService.Features.UserManagement.LoginFeature.Queries.Handler
{
    public class GetUserByIdHandler : BaseHandler<GetUserByIdQuery, RequestResult<UserDto>>
    {
        public GetUserByIdHandler(BaseParameters baseParameters)
            : base(baseParameters)
        {
        }

        public override async Task<RequestResult<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Where(u => u.Id == request.UserId)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Gender = u.Gender
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
            {
                return RequestResult<UserDto>.Failure(AuthService.Common.Enums.ErrorCode.UserNotFound, "User not found.");
            }

            return RequestResult<UserDto>.Success(user);
        }
    }
}
