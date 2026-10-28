using UberSy.Domain.Common.Results;

namespace UberSy.Application.Common.Interfaces;

public interface ISmsSender
{
    Task<Result<Success>> SendAsync(string phoneNumber, string message, CancellationToken ct = default);
}
