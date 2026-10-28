using MediatR;
using Microsoft.Extensions.Logging;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Identity.Commands.ResendEmailCode;

public sealed class ResendEmailCodeCommandHandler(
    IVerificationService verification,
    ILogger<ResendEmailCodeCommandHandler> logger)
    : IRequestHandler<ResendEmailCodeCommand, Result<Updated>>
{
    private readonly IVerificationService _verification = verification;
    private readonly ILogger<ResendEmailCodeCommandHandler> _logger = logger;

    public async Task<Result<Updated>> Handle(ResendEmailCodeCommand request, CancellationToken cancellationToken)
    {
        var result = await _verification.SendEmailCodeAsync(request.Email, cancellationToken);

        if (result.IsError)
        {
            _logger.LogWarning("Resend Email Code Failed: {Error}", result.TopError.Description);
        }

        return Result.Updated;
    }
}