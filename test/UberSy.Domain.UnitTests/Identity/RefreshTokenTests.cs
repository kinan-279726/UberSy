using UberSy.Domain.Identity;
using Xunit;

namespace UberSy.Domain.UnitTests.Identity;

public class RefreshTokenTests
{
    // ============================================================
    // Create Tests
    // ============================================================

    [Fact]
    public void Create_ShouldReturnError_WhenIdIsEmpty()
    {
        var result = RefreshToken.Create(
            id: Guid.Empty,
            token: "valid_token",
            userId: Guid.NewGuid(),
            expiresOnUtc: DateTimeOffset.UtcNow.AddDays(1));

        Assert.False(result.IsSuccess);
        Assert.Equal(RefreshTokenErrors.IdRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenTokenIsNull()
    {
        var result = RefreshToken.Create(
            id: Guid.NewGuid(),
            token: null,
            userId: Guid.NewGuid(),
            expiresOnUtc: DateTimeOffset.UtcNow.AddDays(1));

        Assert.False(result.IsSuccess);
        Assert.Equal(RefreshTokenErrors.TokenRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenTokenIsWhiteSpace()
    {
        var result = RefreshToken.Create(
            id: Guid.NewGuid(),
            token: "   ",
            userId: Guid.NewGuid(),
            expiresOnUtc: DateTimeOffset.UtcNow.AddDays(1));

        Assert.False(result.IsSuccess);
        Assert.Equal(RefreshTokenErrors.TokenRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenUserIdIsEmpty()
    {
        var result = RefreshToken.Create(
            id: Guid.NewGuid(),
            token: "valid_token",
            userId: Guid.Empty,
            expiresOnUtc: DateTimeOffset.UtcNow.AddDays(1));

        Assert.False(result.IsSuccess);
        Assert.Equal(RefreshTokenErrors.UserIdRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenExpiryIsInThePast()
    {
        var result = RefreshToken.Create(
            id: Guid.NewGuid(),
            token: "valid_token",
            userId: Guid.NewGuid(),
            expiresOnUtc: DateTimeOffset.UtcNow.AddDays(-1));

        Assert.False(result.IsSuccess);
        Assert.Equal(RefreshTokenErrors.ExpiryInvalid.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenExpiryIsNow()
    {
        // Testing the exact boundary condition (<= DateTimeOffset.UtcNow)
        var result = RefreshToken.Create(
            id: Guid.NewGuid(),
            token: "valid_token",
            userId: Guid.NewGuid(),
            expiresOnUtc: DateTimeOffset.UtcNow);

        Assert.False(result.IsSuccess);
        Assert.Equal(RefreshTokenErrors.ExpiryInvalid.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_WhenDataIsValid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var token = "valid_refresh_token_123";
        var userId = Guid.NewGuid();
        var expiresOnUtc = DateTimeOffset.UtcNow.AddDays(7);

        // Act
        var result = RefreshToken.Create(id, token, userId, expiresOnUtc);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify properties are set correctly
        Assert.Equal(id, result.Value.Id);
        Assert.Equal(token, result.Value.Token);
        Assert.Equal(userId, result.Value.UserId);
        Assert.Equal(expiresOnUtc, result.Value.ExpiresOnUtc);
    }
}
