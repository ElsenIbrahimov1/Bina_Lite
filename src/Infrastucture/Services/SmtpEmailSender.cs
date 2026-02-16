using Application.Abstracts.Services;
using Application.Options;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Infrastucture.Services;

public sealed class SmtpEmailSender : IEmailSender
{
    private readonly EmailOptions _opt;

    public SmtpEmailSender(IOptions<EmailOptions> options)
    {
        _opt = options.Value;
    }

    public async Task SendAsync(
        string toEmail,
        string subject,
        string? htmlBody,
        string? textBody = null,
        CancellationToken ct = default)
    {
        // ✅ required checks
        if (!_opt.Enabled) return;
        if (string.IsNullOrWhiteSpace(toEmail)) return;

        using var message = new MailMessage();

        message.From = new MailAddress(_opt.FromEmail, _opt.FromName);

        message.To.Add(toEmail);
        message.Subject = subject;

        // Prefer HTML if provided, else plain text
        if (!string.IsNullOrWhiteSpace(htmlBody))
        {
            message.Body = htmlBody;
            message.IsBodyHtml = true;
        }
        else
        {
            message.Body = textBody ?? string.Empty;
            message.IsBodyHtml = false;
        }

        using var client = new SmtpClient(_opt.Host, _opt.Port)
        {
            EnableSsl = _opt.UseSsl
        };

        if (!string.IsNullOrWhiteSpace(_opt.UserName))
        {
            client.Credentials = new NetworkCredential(_opt.UserName, _opt.Password);
        }

        // SmtpClient has no CancellationToken, but we keep ct in signature for consistency
        await client.SendMailAsync(message);
    }
}