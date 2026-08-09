using System.Net;
using System.Net.Mail;
using DotNetCore.CAP;

namespace AuthService.Features.Users.ChangePassword;

public sealed class PasswordChangedEmailSubscriber(
    IConfiguration configuration,
    ILogger<PasswordChangedEmailSubscriber> logger) : ICapSubscribe
{
    [CapSubscribe("auth.password-changed")]
    public async Task SendConfirmationEmail(PasswordChangedEvent message)
    {
        var email = configuration.GetSection("Email");
        var host = email["SmtpHost"];
        var from = email["From"];

        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(from))
            throw new InvalidOperationException("Email:SmtpHost and Email:From must be configured.");

        using var client = new SmtpClient(host, email.GetValue("SmtpPort", 587))
        {
            EnableSsl = email.GetValue("UseSsl", true)
        };

        var username = email["Username"];
        if (!string.IsNullOrWhiteSpace(username))
            client.Credentials = new NetworkCredential(username, email["Password"]);

        using var mail = new MailMessage(from, message.Email)
        {
            Subject = message.Subject,
            Body = "Your password was changed successfully. If you did not make this change, contact support immediately.",
            IsBodyHtml = false
        };

        await client.SendMailAsync(mail);
        logger.LogInformation("Password changed confirmation email sent to user {UserId}.", message.UserId);
    }
}