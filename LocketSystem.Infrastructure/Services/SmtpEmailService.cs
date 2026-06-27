using System.Net;
using System.Net.Mail;
using LocketMini.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LocketMini.Infrastructure.Services;

// ── Settings ──────────────────────────────────────────────────────────────────

public sealed class SmtpSettings
{
    public const string Section = "SmtpSettings";

    public string Host { get; init; } = string.Empty;
    public int Port { get; init; } = 587;
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string FromAddress { get; init; } = string.Empty;
    public string FromName { get; init; } = "LocketMini";
    public bool EnableSsl { get; init; } = true;
}

// ── Service ───────────────────────────────────────────────────────────────────

public sealed class SmtpEmailService : IEmailService
{
    private readonly SmtpSettings _settings;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IOptions<SmtpSettings> options, ILogger<SmtpEmailService> logger)
    {
        _settings = options.Value;
        _logger = logger;
    }

    public async Task SendWelcomeAsync(string toEmail, string username, CancellationToken ct = default)
    {
        var subject = "Chào mừng bạn đến với LocketMini!";
        var body = $"""
            <h2>Xin chào {username}!</h2>
            <p>Cảm ơn bạn đã đăng ký tài khoản LocketMini.</p>
            <p>Hãy bắt đầu chia sẻ những khoảnh khắc của bạn với bạn bè nhé 🎉</p>
            """;

        await SendAsync(toEmail, subject, body, ct);
    }

    public async Task SendPasswordChangedAlertAsync(string toEmail, CancellationToken ct = default)
    {
        var subject = "[LocketMini] Mật khẩu của bạn vừa được thay đổi";
        var body = """
            <p>Mật khẩu tài khoản LocketMini của bạn vừa được thay đổi.</p>
            <p>Nếu bạn không thực hiện hành động này, hãy liên hệ với chúng tôi ngay lập tức.</p>
            """;

        await SendAsync(toEmail, subject, body, ct);
    }

    private async Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken ct)
    {
        try
        {
            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                EnableSsl = _settings.EnableSsl,
                Credentials = new NetworkCredential(_settings.Username, _settings.Password),
            };

            using var message = new MailMessage
            {
                From = new MailAddress(_settings.FromAddress, _settings.FromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true,
            };

            message.To.Add(toEmail);
            await client.SendMailAsync(message, ct);

            _logger.LogInformation("Email đã gửi tới {Email}: {Subject}", toEmail, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Gửi email thất bại tới {Email}", toEmail);
            // Không re-throw: lỗi email không nên làm hỏng luồng chính
        }
    }
}
