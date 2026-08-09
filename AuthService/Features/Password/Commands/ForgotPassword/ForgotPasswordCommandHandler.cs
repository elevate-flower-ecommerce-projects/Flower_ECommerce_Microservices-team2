using AuthService.Common.BaseHandler;
using AuthService.Common.Enums;
using AuthService.Common.Exceptions;
using AuthService.Common.ResultPattern;
using AuthService.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Features.Password.Commands.ForgotPassword
{
    /// <summary>
    /// Finds user by email, generates OTP code, saves OtpVerificationCode to DB, publishes "send email" event via _capPublisher.
    /// </summary>
    public class ForgotPasswordCommandHandler : BaseHandler<ForgotPasswordCommand, RequestResult<bool>>
    {
        public ForgotPasswordCommandHandler(BaseParameters baseParameters) : base(baseParameters)
        {
        }

        public override async Task<RequestResult<bool>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            // 1. Find user by email
            var user = await _context.Users
                .AsTracking()
                .FirstOrDefaultAsync(u => u.Email == request.Email && !u.IsDeleted, cancellationToken);
            if (user == null)
            {
                throw new BusinessException(ErrorCode.BadRequest, "User not found.");
            }
            // 2. Generate 6-digit OTP code
            string otpCode = Random.Shared.Next(100000, 999999).ToString();
            // 3. Save OTP to database
            var otp = new OtpVerificationCode
            {
                Id = _snowflake.CreateId(),
                UserId = user.Id,
                GeneratedCode = otpCode,
                ExpireDate = DateTime.UtcNow.AddMinutes(10), 
                CreatedAt = DateTime.UtcNow
            };
            await _context.OtpVerificationCodes.AddAsync(otp, cancellationToken);
            // 4. Publish integration event via CAP (outbox)
            var emailEvent = new
            {
                Email = user.Email,
                OtpCode = otpCode,
                Purpose = "Reset Password"
            };
            await _capPublisher.PublishAsync("send.otp.email", emailEvent, cancellationToken: cancellationToken);
            return RequestResult<bool>.Success(true, "OTP verification code sent successfully.");
        }
    }
}
