using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Identity.Queries.RefreshTokens;

public sealed record RefreshTokenQuery(string RefreshToken, string ExpiredAccessToken) : IRequest<Result<TokenResponse>>;

public sealed class RefreshTokenQueryHandler(ILogger<RefreshTokenQueryHandler> logger, IIdentityService identityService, IApplicationDbContext context, ITokenProvider tokenProvider)
    : IRequestHandler<RefreshTokenQuery, Result<TokenResponse>>
{
    private readonly ILogger<RefreshTokenQueryHandler> _logger = logger;
    private readonly IIdentityService _identityService = identityService;
    private readonly IApplicationDbContext _context = context;
    private readonly ITokenProvider _tokenProvider = tokenProvider;

    public async Task<Result<TokenResponse>> Handle(RefreshTokenQuery request, CancellationToken cancellationToken)
    {
        var principal = _tokenProvider.GetPrincipalFromExpiredToken(request.ExpiredAccessToken);

        if (principal is null)
        {
            _logger.LogError("Expired access token is not valid");

            return ApplicationErrors.ExpiredAccessTokenInvalid;
        }

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId is null)
        {
            _logger.LogError("Invalid userId claim");

            return ApplicationErrors.UserIdClaimInvalid;
        }

        var userResult = await _identityService.GetUserByIdAsync(Guid.Parse(userId));

        if (userResult.IsError)
        {
            _logger.LogWarning("Get user by id error occurred: {ErrorDescription}", userResult.TopError.Description);
            return userResult.Errors;
        }

        var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == request.RefreshToken && r.UserId == Guid.Parse(userId), cancellationToken);

        if (storedToken is null || storedToken.ExpiresOnUtc < DateTime.UtcNow)
        {
            _logger.LogWarning("Refresh Token Expired Or Not Found For User '{UserId}'.", userId);
            return ApplicationErrors.RefreshTokenExpired;
        }

        var generateTokenResult = await _tokenProvider.GenerateJwtTokenAsync(userResult.Value, cancellationToken);

        if (generateTokenResult.IsError)
        {
            _logger.LogWarning("Generate token error occurred: {ErrorDescription}", generateTokenResult.TopError.Description);
            return generateTokenResult.Errors;
        }

        return generateTokenResult.Value;
    }
}