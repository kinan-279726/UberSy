using System;
using System.Collections.Generic;
using System.Text;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Identity;

namespace UberSy.Test.Common.Auth;

public static class RefreshTokenFactory
{
    public static Result<RefreshToken> CreateRefreshToken(Guid? id = null, string? token = null, Guid? userId = null, DateTimeOffset? expiresOnUtc = null)
    {
        return RefreshToken.Create(
            id ?? Guid.NewGuid(),
            token ?? "sometoken",
            userId ?? Guid.NewGuid(),
            expiresOnUtc ?? DateTime.UtcNow.AddDays(7));
    }
}
