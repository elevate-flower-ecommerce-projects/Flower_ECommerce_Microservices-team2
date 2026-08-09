using AuthService.Common.BaseHandler;
using AuthService.Common.Enums;
using AuthService.Common.Helpers;
using AuthService.Common.ResultPattern;
using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Features.Users.ChangePassword;

public sealed class ChangePasswordCommandHandler(
    BaseParameters baseParameters,
    ICapPublisher publisher)
    : BaseHandler<ChangePasswordCommand, RequestResult<bool>>(baseParameters)
{
    public override async Task<RequestResult<bool>> Handle(
        ChangePasswordCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var user = await _context.Users
            .FirstOrDefaultAsync(candidate => candidate.Id == userId, cancellationToken);

        if (user is null)
            return RequestResult<bool>.Failure(ErrorCode.Unauthorized, "Authenticated user was not found.");

        if (!PasswordHasher.Verify(request.CurrentPassword, user.Password))
            return RequestResult<bool>.Failure(ErrorCode.InvalidInput, "Current password is incorrect");

        if (!IsComplexPassword(request.NewPassword))
            return RequestResult<bool>.Failure(ErrorCode.InvalidInput, "New password does not meet the password complexity requirements.");

        if (!string.Equals(request.NewPassword, request.ConfirmNewPassword, StringComparison.Ordinal))
            return RequestResult<bool>.Failure(ErrorCode.InvalidInput, "Passwords do not match.");

        if (string.Equals(request.CurrentPassword, request.NewPassword, StringComparison.Ordinal))
            return RequestResult<bool>.Failure(ErrorCode.InvalidInput, "New password must differ from the current password.");

        user.Password = PasswordHasher.Hash(request.NewPassword);
        user.PasswordChangedAt = DateTime.UtcNow;
        _context.Users.Attach(user);
        _context.Entry(user).State = EntityState.Modified;

        await _context.RefreshTokens
            .Where(token => token.UserId == userId)
            .ExecuteUpdateAsync(update => update.SetProperty(token => token.IsDeleted, true), cancellationToken);

        await publisher.PublishAsync(
            "auth.password-changed",
            new PasswordChangedEvent(user.Id, user.Email, "Your password was changed"),
            cancellationToken: cancellationToken);

        return RequestResult<bool>.Success(true, "Password changed successfully.");
    }

    private static bool IsComplexPassword(string password) =>
        !string.IsNullOrEmpty(password) &&
        password.Length >= 6 &&
        password.Any(char.IsUpper) &&
        password.Any(char.IsLower) &&
        password.Any(char.IsDigit) &&
        password.Any("!@#$%^&*()_+-=[]{};':\"|,.<>/?".Contains);
}