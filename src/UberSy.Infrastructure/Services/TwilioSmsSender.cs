using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;
using UberSy.Infrastructure.Settings;

namespace UberSy.Infrastructure.Services;

public sealed class TwilioSmsSender(IOptions<SmsSettings> options, ILogger<TwilioSmsSender> logger) : ISmsSender
{
    private readonly SmsSettings _settings = options.Value;
    private readonly ILogger<TwilioSmsSender> _logger = logger;

    public async Task<Result<Success>> SendAsync(string phoneNumber, string message, CancellationToken ct = default)
    {
        try
        {
            var client = new TwilioRestClient(_settings.AccountSid, _settings.AuthToken);

            var response = await MessageResource.CreateAsync(
                to: new PhoneNumber(phoneNumber),
                from: new PhoneNumber(_settings.FromNumber),
                body: message,
                client: client);

            if (response.ErrorCode is not null)
            {
                _logger.LogError("Twilio Rejected SMS To '{To}': {Code} {Message}", phoneNumber, response.ErrorCode, response.ErrorMessage);
                return ApplicationErrors.SmsSendFailed;
            }

            return Result.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed To Send SMS To '{To}'.", phoneNumber);
            return ApplicationErrors.SmsSendFailed;
        }
    }
}
