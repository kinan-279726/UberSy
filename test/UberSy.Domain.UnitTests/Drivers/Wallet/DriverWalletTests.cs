using UberSy.Domain.Drivers;
using UberSy.Domain.Drivers.Wallet;
using Xunit;

namespace UberSy.Domain.UnitTests.Drivers.Wallet;

public class DriverWalletTests
{
    // ============================================================
    // Helper Methods
    // ============================================================
    private static TbDriverWallet CreateValidWallet()
    {
        return TbDriverWallet.Create(
            id: Guid.NewGuid(),
            driverId: Guid.NewGuid()
        ).Value;
    }

    // ============================================================
    // Create Tests
    // ============================================================

    [Fact]
    public void Create_ShouldReturnError_WhenDriverIdIsEmpty()
    {
        var result = TbDriverWallet.Create(
            id: Guid.NewGuid(),
            driverId: Guid.Empty);

        Assert.False(result.IsSuccess);
        Assert.Equal(DriverErrors.UserIdRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_AndInitializeBalanceToZero()
    {
        // Arrange
        var id = Guid.NewGuid();
        var driverId = Guid.NewGuid();

        // Act
        var result = TbDriverWallet.Create(id, driverId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(id, result.Value.Id);
        Assert.Equal(driverId, result.Value.DriverId);
        Assert.Equal(0m, result.Value.Balance); // Verify initial balance is zero
    }

    // ============================================================
    // Deposit Tests
    // ============================================================

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Deposit_ShouldReturnError_WhenAmountIsZeroOrNegative(decimal invalidAmount)
    {
        var wallet = CreateValidWallet();

        var result = wallet.Deposit(invalidAmount);

        Assert.False(result.IsSuccess);
        Assert.Equal(WalletErrors.AmountMustBePositive.Code, result.TopError.Code);
    }

    [Fact]
    public void Deposit_ShouldReturnSuccess_AndIncreaseBalance()
    {
        // Arrange
        var wallet = CreateValidWallet();
        var depositAmount = 100.50m;

        // Act
        var result = wallet.Deposit(depositAmount);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(100.50m, wallet.Balance);
    }

    // ============================================================
    // Deduct Tests
    // ============================================================

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Deduct_ShouldReturnError_WhenAmountIsZeroOrNegative(decimal invalidAmount)
    {
        var wallet = CreateValidWallet();

        var result = wallet.Deduct(invalidAmount);

        Assert.False(result.IsSuccess);
        Assert.Equal(WalletErrors.AmountMustBePositive.Code, result.TopError.Code);
    }

    [Fact]
    public void Deduct_ShouldReturnSuccess_AndDecreaseBalance()
    {
        // Arrange
        var wallet = CreateValidWallet();
        wallet.Deposit(200m); // Setup balance first
        var deductAmount = 50.50m;

        // Act
        var result = wallet.Deduct(deductAmount);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(149.50m, wallet.Balance);
    }

    // ============================================================
    // Edge Case Tests
    // ============================================================

    [Fact]
    public void Deduct_ShouldAllowBalanceToGoNegative()
    {
        // Arrange
        var wallet = CreateValidWallet();
        wallet.Deposit(10m);

        // Act
        var result = wallet.Deduct(50m);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(-40m, wallet.Balance);
    }
}
