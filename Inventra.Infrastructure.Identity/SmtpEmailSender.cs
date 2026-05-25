using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Inventra.Infrastructure.Identity;

internal sealed class SmtpEmailSender(IOptions<MailOptions> options)
{
    private readonly MailOptions _options = options.Value;

    public async Task SendAsync(string toEmail, string subject, string htmlBody,
        CancellationToken cancellationToken = default)
    {
        var message = CreateMessage(toEmail, subject, htmlBody);
        using var client = new SmtpClient();

        await client.ConnectAsync(_options.Host, _options.Port, SecureSocketOptions.StartTls, cancellationToken);
        await client.AuthenticateAsync(_options.Username, _options.Password, cancellationToken);
        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }

    private MimeMessage CreateMessage(string toEmail, string subject, string htmlBody)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_options.FromName, _options.FromEmail));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;
        message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

        return message;
    }
}
