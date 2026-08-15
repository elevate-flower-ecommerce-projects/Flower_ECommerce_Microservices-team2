using Catalog_Service.Common.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Catalog_Service.Common.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> options)
    {
        _settings = options.Value;
    }

    // ── General-purpose sender ───────────────────────────────────────────────────
    public async Task SendAsync(
        string toEmail,
        string toName,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default)
    {
        var message = BuildMessage(toEmail, toName, subject, htmlBody);
        await DispatchAsync(message, cancellationToken);
    }

    // ── OTP-specific sender ──────────────────────────────────────────────────────
    public async Task SendOtpAsync(
        string toEmail,
        string toName,
        string otp,
        CancellationToken cancellationToken = default)
    {
        var message = BuildMessage(
            toEmail,
            toName,
            "Your Verification Code",
            BuildOtpEmailBody(toName, otp));

        await DispatchAsync(message, cancellationToken);
    }

    // ── Private helpers ──────────────────────────────────────────────────────────

    private MimeMessage BuildMessage(string toEmail, string toName, string subject, string htmlBody)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        message.To.Add(new MailboxAddress(toName, toEmail));
        message.Subject = subject;
        message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();
        return message;
    }

    private async Task DispatchAsync(MimeMessage message, CancellationToken cancellationToken)
    {
        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls, cancellationToken);
        await smtp.AuthenticateAsync(_settings.SenderEmail, _settings.Password, cancellationToken);
        await smtp.SendAsync(message, cancellationToken);
        await smtp.DisconnectAsync(true, cancellationToken);
    }

    private static string BuildOtpEmailBody(string fullName, string otp) => $"""
        <div style="font-family:Arial,sans-serif;max-width:520px;margin:auto;padding:32px;
                    border:1px solid #e5e7eb;border-radius:12px;background:#ffffff;">
          <h2 style="color:#1d4ed8;margin-bottom:8px;">Email Verification</h2>
          <p style="color:#374151;">Hi <strong>{fullName}</strong>,</p>
          <p style="color:#374151;">Use the code below to verify your email address.
             It expires in <strong>5 minutes</strong>.</p>
          <div style="text-align:center;margin:28px 0;">
            <span style="display:inline-block;font-size:36px;font-weight:700;
                         letter-spacing:12px;color:#1d4ed8;
                         background:#eff6ff;padding:16px 28px;border-radius:8px;">
              {otp}
            </span>
          </div>
          <p style="color:#6b7280;font-size:13px;">
            If you did not request this, please ignore this email.
          </p>
        </div>
        """;
}
