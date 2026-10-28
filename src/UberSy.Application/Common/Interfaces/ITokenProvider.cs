using System.Security.Claims;
using UberSy.Application.Features.Identity;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Common.Interfaces;

public interface ITokenProvider
{
    Task<Result<TokenResponse>> GenerateJwtTokenAsync(ApplicationUserDto user, CancellationToken ct = default);

    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}