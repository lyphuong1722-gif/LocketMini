namespace LocketMini.Application.Common.Interfaces;

/// <summary>
/// Gửi email — implementation ở Infrastructure (SendGrid, SMTP, v.v.)
/// </summary>
public interface IEmailService
{
    Task SendWelcomeAsync(string toEmail, string username, CancellationToken ct = default);
    Task SendPasswordChangedAlertAsync(string toEmail, CancellationToken ct = default);
}
