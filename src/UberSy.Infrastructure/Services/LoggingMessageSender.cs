using Microsoft.Extensions.Logging;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;

namespace UberSy.Infrastructure.Services;

public sealed class LoggingMessageSender(ILogger<LoggingMessageSender> logger) : IEmailSender, ISmsSender
{
    private readonly ILogger<LoggingMessageSender> _logger = logger;

    public Task<Result<Success>> SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
    {
        _logger.LogWarning("[FAKE EMAIL] To: {To} | Subject: {Subject} | Body: {Body}", to, subject, htmlBody);
        return Task.FromResult<Result<Success>>(Result.Success);
    }

    public Task<Result<Success>> SendAsync(string phoneNumber, string message, CancellationToken ct = default)
    {
        _logger.LogWarning("[FAKE SMS] To: {To} | Message: {Message}", phoneNumber, message);
        return Task.FromResult<Result<Success>>(Result.Success);
    }
}