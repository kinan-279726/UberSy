using MediatR;
using Microsoft.Extensions.Logging;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Identity.Commands.Register;

public sealed class RegisterCommandHandler(
    IVerificationService verification,
    IIdentityService identityService,
    ILogger<RegisterCommandHandler> logger)
    : IRequestHandler<RegisterCommand, Result<ApplicationUserDto>>
{
    private readonly IVerificationService _verification = verification;
    private readonly IIdentityService _identityService = identityService;
    private readonly ILogger<RegisterCommandHandler> _logger = logger;

    public async Task<Result<ApplicationUserDto>> Handle(RegisterCommand request, CancellationToken ct)
    {
        var result = await _identityService.RegisterAsync(
            request.Email, request.Password, request.NameAr, request.NameEn, request.Phone, role: Domain.Identity.Roles.Customer);

        if (result.IsError)
        {
            _logger.LogError("Registration Failed For '{Email}': {Error}", request.Email, result.TopError.Description);
            return result.Errors;
        }

        _logger.LogInformation("User '{UserId}' Registered Successfully.", result.Value.UserId);

        var sendResult = await _verification.SendEmailCodeAsync(result.Value.Email, ct);

        if (sendResult.IsError)
        {
            _logger.LogWarning("User '{UserId}' Registered But The Verification Email Failed: {Error}",
                result.Value.UserId, sendResult.TopError.Description);
        }

        return result.Value;
    }
}