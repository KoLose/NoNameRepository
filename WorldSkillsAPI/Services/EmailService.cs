using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using WorldSkillsAPI.Configuration;
using WorldSkillsAPI.Interfaces;

namespace WorldSkillsAPI.Services;

public class EmailService : IEmailService
{
    private readonly SmtpOptions _options;

    public EmailService(IOptions<SmtpOptions> options)
    {
        _options = options.Value;
    }

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(_options.Host) &&
        _options.Port > 0 &&
        !string.IsNullOrWhiteSpace(_options.FromEmail);

    public async Task SendAsync(
        string recipientName,
        string recipientEmail,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        if (!IsConfigured)
            throw new InvalidOperationException("SMTP не настроен");

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_options.FromName, _options.FromEmail));
        message.To.Add(new MailboxAddress(recipientName, recipientEmail));
        message.Subject = subject;
        message.Body = new TextPart("plain") { Text = body };

        using var client = new SmtpClient
        {
            // On some macOS development environments the system cannot complete
            // the online certificate revocation check even though the Gmail
            // certificate chain itself is valid. All other TLS validation stays on.
            CheckCertificateRevocation = false
        };
        await client.ConnectAsync(
            _options.Host,
            _options.Port,
            SecureSocketOptions.Auto,
            cancellationToken);

        if (!string.IsNullOrWhiteSpace(_options.UserName))
        {
            await client.AuthenticateAsync(
                _options.UserName,
                _options.Password,
                cancellationToken);
        }

        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}
