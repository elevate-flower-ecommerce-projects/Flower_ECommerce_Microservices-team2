using AuthService.Common.BaseHandler;
using AuthService.Common.Enums;
using AuthService.Common.Exceptions;
using AuthService.Common.Helpers;
using AuthService.Common.ResultPattern;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Features.Password.Commands.ResetPassword
{
    /// <summary>
    /// Finds user by email, finds valid OTP, checks code + expiry, hashes new password, updates user.Password,
    /// soft-deletes the OTP. Returns RequestResult<bool>.Success(true) or throws BusinessException.
    /// </summary>
    public class ResetPasswordCommandHandler : BaseHandler<ResetPasswordCommand, RequestResult<bool>>
    {
        public ResetPasswordCommandHandler(BaseParameters baseParameters) : base(baseParameters)
        {
        }

        public override async Task<RequestResult<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            // 1. Find user by email
            var user = await _context.Users
                .AsTracking()
                .FirstOrDefaultAsync(u => u.Email == request.Email && !u.IsDeleted, cancellationToken);
            if (user == null)
            {
                throw new BusinessException(ErrorCode.BadRequest, "User not found.");
            }

            // 2. Validate OTP
            var otp = await _context.OtpVerificationCodes
                .AsTracking()
                .FirstOrDefaultAsync(o => o.UserId == user.Id
                                          && o.GeneratedCode == request.OtpCode
                                          && o.ExpireDate > DateTime.UtcNow
                                          && !o.IsDeleted, cancellationToken);
            if (otp == null)
            {
                throw new BusinessException(ErrorCode.BadRequest, "Invalid or expired OTP code.");
            }

            // 3. Update password
            user.Password = PasswordHasher.Hash(request.NewPassword);
            _context.Users.Update(user);

            // 4. Soft-delete OTP code (setting IsDeleted = true)
            otp.IsDeleted = true;
            _context.OtpVerificationCodes.Update(otp);
            return RequestResult<bool>.Success(true, "Password has been reset successfully.");
        }
    }
}
