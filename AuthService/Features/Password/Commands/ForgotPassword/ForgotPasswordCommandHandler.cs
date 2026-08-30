using AuthService.Common.BaseHandler;
using AuthService.Common.Enums;
using AuthService.Common.Exceptions;
using AuthService.Common.ResultPattern;
using AuthService.Common.Services;
using AuthService.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Features.Password.Commands.ForgotPassword
{
    /// <summary>
    /// Finds user by email, generates OTP code, saves OtpVerificationCode to DB, sends OTP email, and publishes event.
    /// </summary>
    public class ForgotPasswordCommandHandler : BaseHandler<ForgotPasswordCommand, RequestResult<bool>>
    {
        private readonly IEmailService _emailService;

        public ForgotPasswordCommandHandler(BaseParameters baseParameters, IEmailService emailService) : base(baseParameters)
        {
            _emailService = emailService;
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
            await _context.SaveChangesAsync(cancellationToken);

            // 4. Send email directly via IEmailService
            await _emailService.SendOtpAsync(user.Email, user.FullName, otpCode, cancellationToken);

            // 5. Publish integration event via CAP (outbox)
            try
            {
                var emailEvent = new
                {
                    Email = user.Email,
                    OtpCode = otpCode,
                    Purpose = "Reset Password"
                };
                await _capPublisher.PublishAsync("send.otp.email", emailEvent, cancellationToken: cancellationToken);
            }
            catch
            {
                // Ignore CAP publish errors if running without message broker
            }

            return RequestResult<bool>.Success(true, "OTP verification code sent successfully.");
        }
    }
}
