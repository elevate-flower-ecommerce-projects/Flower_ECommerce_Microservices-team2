using AuthService.Common.BaseHandler;
using AuthService.Common.Enums;
using AuthService.Common.ResultPattern;
using AuthService.Entities;
using AuthService.Features.Password.Dto;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Features.Password.Commands.VerifyOtp
{
    public class VerifyOtpCommandHandler : BaseHandler<VerifyOtpCommand, RequestResult<VerifyOtpResponse>>
    {
        public VerifyOtpCommandHandler(BaseParameters baseParameters) : base(baseParameters)
        {
        }

        public override async Task<RequestResult<VerifyOtpResponse>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .AsTracking()
                .FirstOrDefaultAsync(u => u.Email == request.Email && !u.IsDeleted, cancellationToken);

            if (user == null)
            {
                return RequestResult<VerifyOtpResponse>.Failure(ErrorCode.BadRequest, "Invalid or expired OTP code.");
            }

            var otp = await _context.OtpVerificationCodes
                .AsTracking()
                .FirstOrDefaultAsync(o => o.UserId == user.Id
                                          && o.GeneratedCode == request.Otp
                                          && o.ExpireDate > DateTime.UtcNow
                                          && !o.IsDeleted, cancellationToken);

            if (otp == null)
            {
                return RequestResult<VerifyOtpResponse>.Failure(ErrorCode.BadRequest, "Invalid or expired OTP code.");
            }

            // Soft-delete the 6-digit OTP so it cannot be reused
            otp.IsDeleted = true;
            _context.OtpVerificationCodes.Update(otp);

            // Generate short-lived reset authorization token (15 mins)
            var resetToken = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
            var expiresAtUtc = DateTime.UtcNow.AddMinutes(15);

            var resetTokenEntity = new OtpVerificationCode
            {
                Id = _snowflake.CreateId(),
                UserId = user.Id,
                GeneratedCode = resetToken,
                ExpireDate = expiresAtUtc,
                CreatedAt = DateTime.UtcNow
            };

            await _context.OtpVerificationCodes.AddAsync(resetTokenEntity, cancellationToken);

            return RequestResult<VerifyOtpResponse>.Success(new VerifyOtpResponse
            {
                ResetToken = resetToken,
                ExpiresAtUtc = expiresAtUtc
            }, "Reset code verified successfully.");
        }
    }
}
