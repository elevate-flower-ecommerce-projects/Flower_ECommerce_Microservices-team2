namespace AuthService.Features.Users.ChangePassword;

public sealed record PasswordChangedEvent(long UserId, string Email, string Subject);