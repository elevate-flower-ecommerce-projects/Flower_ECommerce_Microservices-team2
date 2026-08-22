using System.Text.RegularExpressions;
using AuthService.Common.BaseHandler;
using AuthService.Common.Enums;
using AuthService.Common.Helpers;
using AuthService.Common.ResultPattern;
using AuthService.Common.Services;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Features.Users.ChangePassword;

public sealed class ChangePasswordCommandHandler(
    BaseParameters baseParameters,
    IEmailService emailService)
    : BaseHandler<ChangePasswordCommand, RequestResult<bool>>(baseParameters)
{
    public override async Task<RequestResult<bool>> Handle(
        ChangePasswordCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .AsTracking()
            .SingleOrDefaultAsync(user => user.Id == _currentUserService.UserId, cancellationToken);

        if (user is null)
            return RequestResult<bool>.Failure(ErrorCode.UserNotFound, "User not found.");

        // Verify this first so an invalid current password never reveals new-password details.
        if (string.IsNullOrEmpty(request.CurrentPassword) ||
            !PasswordHasher.Verify(request.CurrentPassword, user.Password))
            return RequestResult<bool>.Failure(ErrorCode.InvalidCurrentPassword, "Current password is incorrect.");

        if (request.NewPassword != request.ConfirmNewPassword)
            return RequestResult<bool>.Failure(ErrorCode.PasswordMismatch, "New password and confirmation do not match.");

        if (!IsComplexPassword(request.NewPassword))
            return RequestResult<bool>.Failure(
                ErrorCode.InvalidInput,
                "Password must be at least 6 characters long and contain uppercase, lowercase, digit, and special characters.");

        if (PasswordHasher.Verify(request.NewPassword, user.Password))
            return RequestResult<bool>.Failure(
                ErrorCode.PasswordReuse,
                "New password must be different from the current password.");

        user.Password = PasswordHasher.Hash(request.NewPassword);

        await _context.RefreshTokens
            .Where(token => token.UserId == user.Id && !token.IsDeleted)
            .ExecuteUpdateAsync(
                updates => updates.SetProperty(token => token.IsDeleted, true),
                cancellationToken);

        //await emailService.SendAsync(
        //    user.Email,
        //    user.FullName,
        //    "Your password was changed",
        //    $"<p>Hi <strong>{user.FullName}</strong>,</p><p>Your password was changed successfully. If you did not make this change, please contact support immediately.</p>",
        //    cancellationToken);

        return RequestResult<bool>.Success(true, "Password changed successfully. Please sign in again.");
    }

    private static bool IsComplexPassword(string password) =>
        !string.IsNullOrWhiteSpace(password) &&
        password.Length >= 6 &&
        Regex.IsMatch(password, "[A-Z]") &&
        Regex.IsMatch(password, "[a-z]") &&
        Regex.IsMatch(password, "[0-9]") &&
        password.Any(character => "!@#$%^&*()_+-=[]{};':\"\\|,.<>/?".Contains(character));
}