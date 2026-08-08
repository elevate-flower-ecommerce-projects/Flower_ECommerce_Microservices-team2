using AuthService.Common.BaseHandler;
using AuthService.Common.Enums;
using AuthService.Common.ResultPattern;
using AuthService.Entities;
using AuthService.Features.CustomerRegistration.Command.Verification;
using AuthService.Features.CustomerRegistration.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AuthService.Features.CustomerRegistration.Handler.VerificationHandler
{
    public class VerificationEmailHandler : BaseHandler<VerifyEmailCommand, RequestResult<bool>>
    {
        public VerificationEmailHandler(BaseParameters baseParameters) : base(baseParameters)
        {
        }

        public override async Task<RequestResult<bool>> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
        {

            var otpResult = await _mediator.Send(new GetOtpByUserIdAndCodeQuery(request.UserId, request.code), cancellationToken);

            if (otpResult.Data == null)
                return RequestResult<bool>.Failure(ErrorCode.BadRequest, "Invalid verification code.");

            if (otpResult.Data.ExpireDate < DateTime.UtcNow)
                return RequestResult<bool>.Failure(ErrorCode.BadRequest, "Verification code has expired. Please request a new code.");

            var user = new User();

            user.IsEmailConfirmed = true;

            EntityEntry<User> entityEntry = _context.Users.Attach(user);

            entityEntry.Property(u => u.IsEmailConfirmed).IsModified = true;


            await _context.SaveChangesAsync(cancellationToken);

            return RequestResult<bool>.Success(true, "Email verified successfully.");
        }
    }
}
