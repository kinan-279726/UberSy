using UberSy.Domain.Common.Results;

namespace UberSy.Application.Common.Interfaces;

public interface IEmailSender
{
    Task<Result<Success>> SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default);
}
