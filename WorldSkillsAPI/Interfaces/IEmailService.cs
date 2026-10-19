namespace WorldSkillsAPI.Interfaces;

public interface IEmailService
{
    bool IsConfigured { get; }

    Task SendAsync(
        string recipientName,
        string recipientEmail,
        string subject,
        string body,
        CancellationToken cancellationToken = default);
}
