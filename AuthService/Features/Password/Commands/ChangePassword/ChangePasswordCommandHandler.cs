using AuthService.Common.BaseHandler;
using AuthService.Common.Enums;
using AuthService.Common.Exceptions;
using AuthService.Common.Helpers;
using AuthService.Common.ResultPattern;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Features.Password.Commands.ChangePassword;

public class ChangePasswordCommandHandler : BaseHandler<ChangePasswordCommand, RequestResult<bool>>
{
    public ChangePasswordCommandHandler(BaseParameters baseParameters) : base(baseParameters)
    {
    }
    public override async Task<RequestResult<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        // 1. Get current logged-in user
        long userId = _currentUserService.UserId;
        if (userId == 0)
        {
            throw new BusinessException(ErrorCode.Unauthorized, "User is not authenticated.");
        }
        // 2. Find user in database
        var user = await _context.Users
            .AsTracking()
            .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken);
        if (user == null)
        {
            throw new BusinessException(ErrorCode.BadRequest, "User not found.");
        }
        // 3. Verify current password
        if (!PasswordHasher.Verify(request.CurrentPassword, user.Password))
        {
            throw new BusinessException(ErrorCode.BadRequest, "Incorrect current password.");
        }
        // 4. Hash and save new password
        user.Password = PasswordHasher.Hash(request.NewPassword);
        _context.Users.Update(user);
        return RequestResult<bool>.Success(true, "Password changed successfully.");
    }
}