
using AuthService.Common.BaseHandler;
using AuthService.Common.ResultPattern;
using AuthService.Features.CustomerRegistration.Query;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Features.CustomerRegistration.Handler.QueryHandler
{
    public class CheckUserExistsQueryHandler : BaseHandler<CheckUserExistsQuery, RequestResult<bool>>
    {
        public CheckUserExistsQueryHandler(BaseParameters baseParameters) : base(baseParameters)
        {
        }

        public override async Task<RequestResult<bool>> Handle(CheckUserExistsQuery request, CancellationToken cancellationToken)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Email == request.Email || u.PhoneNumber == request.PhoneNumber, cancellationToken);

            if (userExists)
                return RequestResult<bool>.Success(true, "User with this email or phone number already exists.");

            return RequestResult<bool>.Success(false);
        }
    }
}
