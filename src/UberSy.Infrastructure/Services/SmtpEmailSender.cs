using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;
using UberSy.Infrastructure.Settings;

namespace UberSy.Infrastructure.Services;

public sealed class SmtpEmailSender(IOptions<EmailSettings> options, ILogger<SmtpEmailSender> logger) : IEmailSender
{
    private readonly EmailSettings _settings = options.Value;
    private readonly ILogger<SmtpEmailSender> _logger = logger;

    public async Task<Result<Success>> SendAsync(string to, string subject, string htmlBody, CancellationToken cancellation = default)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromAddress));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls, cancellation);
            await client.AuthenticateAsync(_settings.Username, _settings.Password, cancellation);
            await client.SendAsync(message, cancellation);
            await client.DisconnectAsync(true, cancellation);

            return Result.Success;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed To Send Email To '{To}'.", to);
            return ApplicationErrors.EmailSendFailed;
        }
    }
}
