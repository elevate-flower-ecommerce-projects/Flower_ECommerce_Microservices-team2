namespace AuthService.Common.Services;

public interface IEmailService
{
    /// <summary>General-purpose HTML email sender.</summary>
    Task SendAsync(string toEmail, string toName, string subject, string htmlBody, CancellationToken cancellationToken = default);

    /// <summary>Sends the branded OTP verification email with the built-in HTML template.</summary>
    Task SendOtpAsync(string toEmail, string toName, string otp, CancellationToken cancellationToken = default);
}
