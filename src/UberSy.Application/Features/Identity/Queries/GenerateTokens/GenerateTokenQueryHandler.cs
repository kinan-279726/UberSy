using MediatR;
using Microsoft.Extensions.Logging;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Identity.Queries.GenerateTokens;

public sealed class GenerateTokenQueryHandler(ILogger<GenerateTokenQueryHandler> logger, IIdentityService identityService, ITokenProvider tokenProvider)
    : IRequestHandler<GenerateTokenQuery, Result<TokenResponse>>
{
    private readonly ILogger<GenerateTokenQueryHandler> _logger = logger;
    private readonly IIdentityService _identityService = identityService;
    private readonly ITokenProvider _tokenProvider = tokenProvider;

    public async Task<Result<TokenResponse>> Handle(GenerateTokenQuery request, CancellationToken cancellationToken)
    {
        var userResult = await _identityService.AuthenticateAsync(request.Email, request.Password);

        if (userResult.IsError)
        {
            _logger.LogWarning("Login Failed For '{Email}'.", request.Email);
            return userResult.Errors;
        }

        var generateTokenResult = await _tokenProvider.GenerateJwtTokenAsync(userResult.Value, cancellationToken);

        if (generateTokenResult.IsError)
        {
            _logger.LogError("Token Generation Failed For '{Email}': {Error}", request.Email, generateTokenResult.TopError.Description);
            return generateTokenResult.Errors;
        }

        return generateTokenResult.Value;
    }
}