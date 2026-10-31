using UberSy.Domain.Drivers.Wallet;
using UberSy.Domain.Drivers.Wallet.Enums;
using Xunit;

namespace UberSy.Domain.UnitTests.Drivers.Wallet;

public class DriverWalletTransactionTests
{
    // ============================================================
    // Helper Methods
    // ============================================================
    private static TbDriverWalletTransaction CreateValidTopUpTransaction()
    {
        return TbDriverWalletTransaction.Create(
            id: Guid.NewGuid(),
            driverWalletId: Guid.NewGuid(),
            serviceRequestId: null,
            processedByUserId: Guid.NewGuid(), // Required for TopUp
            amount: 100m,
            type: WalletTransactionType.TopUp
        ).Value;
    }

    // ============================================================
    // Create Tests - General Validation
    // ============================================================

    [Fact]
    public void Create_ShouldReturnError_WhenDriverWalletIdIsEmpty()
    {
        var result = TbDriverWalletTransaction.Create(
            Guid.NewGuid(), Guid.Empty, null, Guid.NewGuid(), 100m, WalletTransactionType.TopUp);

        Assert.False(result.IsSuccess);
        Assert.Equal(WalletErrors.WalletNotFound.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenAmountIsZero()
    {
        var result = TbDriverWalletTransaction.Create(
            Guid.NewGuid(), Guid.NewGuid(), null, Guid.NewGuid(), 0m, WalletTransactionType.TopUp);

        Assert.False(result.IsSuccess);
        Assert.Equal(WalletErrors.AmountMustBePositive.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenTypeIsInvalid()
    {
        var result = TbDriverWalletTransaction.Create(
            Guid.NewGuid(), Guid.NewGuid(), null, Guid.NewGuid(), 100m, (WalletTransactionType)222);

        Assert.False(result.IsSuccess);
        Assert.Equal(WalletErrors.AmountMustBePositive.Code, result.TopError.Code);
    }

    // ============================================================
    // Create Tests - Type Specific Rules (TopUp)
    // ============================================================

    [Fact]
    public void Create_ShouldReturnError_WhenTopUpHasNoProcessor()
    {
        var result = TbDriverWalletTransaction.Create(
            id: Guid.NewGuid(),
            driverWalletId: Guid.NewGuid(),
            serviceRequestId: null,
            processedByUserId: null, // Missing Processor
            amount: 100m,
            type: WalletTransactionType.TopUp);

        Assert.False(result.IsSuccess);
        Assert.Equal(WalletErrors.TopUpRequiresProcessor.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_WhenTopUpDataIsValid()
    {
        var result = TbDriverWalletTransaction.Create(
            id: Guid.NewGuid(),
            driverWalletId: Guid.NewGuid(),
            serviceRequestId: null,
            processedByUserId: Guid.NewGuid(),
            amount: 100m,
            type: WalletTransactionType.TopUp);

        Assert.True(result.IsSuccess);
        Assert.Equal(WalletTransactionType.TopUp, result.Value.Type);
    }

    // ============================================================
    // Create Tests - Type Specific Rules (Deductions)
    // ============================================================

    [Theory]
    [InlineData(WalletTransactionType.PlatformCommissionDeduction)]
    [InlineData(WalletTransactionType.CancellationCompensation)]
    public void Create_ShouldReturnError_WhenDeductionHasNoServiceRequest(WalletTransactionType deductionType)
    {
        var result = TbDriverWalletTransaction.Create(
            id: Guid.NewGuid(),
            driverWalletId: Guid.NewGuid(),
            serviceRequestId: null, // Missing ServiceRequest
            processedByUserId: null,
            amount: 50m,
            type: deductionType);

        Assert.False(result.IsSuccess);
        Assert.Equal(WalletErrors.DeductionRequiresServiceRequest.Code, result.TopError.Code);
    }

    [Theory]
    [InlineData(WalletTransactionType.PlatformCommissionDeduction)]
    [InlineData(WalletTransactionType.CancellationCompensation)]
    public void Create_ShouldReturnSuccess_WhenDeductionDataIsValid(WalletTransactionType deductionType)
    {
        var result = TbDriverWalletTransaction.Create(
            id: Guid.NewGuid(),
            driverWalletId: Guid.NewGuid(),
            serviceRequestId: Guid.NewGuid(), // Provided
            processedByUserId: null,
            amount: 50m,
            type: deductionType);

        Assert.True(result.IsSuccess);
        Assert.Equal(deductionType, result.Value.Type);
    }
}
