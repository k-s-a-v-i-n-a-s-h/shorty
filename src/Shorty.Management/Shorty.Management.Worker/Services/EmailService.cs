using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using Shorty.Management.Worker.Models;
using Shorty.Management.Worker.Options;

namespace Shorty.Management.Worker.Services;

internal sealed class EmailService
{
    private readonly SmtpOptions _options;

    public EmailService(IOptions<SmtpOptions> options)
    {
        _options = options.Value;
    }

    public async Task<(object, Func<Task>)> ConnectAsync(CancellationToken token = default)
    {
        var client = new SmtpClient();
        await client.ConnectAsync(_options.Host, _options.Port, MailKit.Security.SecureSocketOptions.StartTls, token);
        await client.AuthenticateAsync(_options.Username, _options.Password, token);

        async Task cleanup()
        {
            try
            {
                if (client.IsConnected)
                    await client.DisconnectAsync(true, CancellationToken.None);
            }
            finally
            {
                client.Dispose();
            }
        }

        return (client, cleanup);
    }

    public async Task SendAsync(object client, EmailMessage message, CancellationToken token = default)
    {
        if (client is not SmtpClient smtpClient)
            throw new ArgumentException("Invalid client type");

        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(message.From.Name, message.From.Email));
        email.To.Add(new MailboxAddress("", message.To));
        email.Subject = message.Subject;
        email.Body = new TextPart("html") { Text = message.HtmlBody };

        foreach (var cc in message.Cc)
            email.Cc.Add(new MailboxAddress("", cc));

        foreach (var bcc in message.Bcc)
            email.Bcc.Add(new MailboxAddress("", bcc));

        await smtpClient.SendAsync(email, token);
    }
}
