using UberSy.Domain.Common.Results;

namespace UberSy.Application.Common.Interfaces;

public interface IVerificationService
{
    Task<Result<Updated>> SendEmailCodeAsync(string email, CancellationToken ct = default);
    Task<Result<Updated>> ConfirmEmailAsync(string email, string code);
    Task<Result<Updated>> SendPhoneCodeAsync(Guid userId, CancellationToken ct = default);
    Task<Result<Updated>> ConfirmPhoneAsync(Guid userId, string code);
}