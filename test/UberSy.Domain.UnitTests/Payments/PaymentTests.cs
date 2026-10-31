using UberSy.Domain.Payments;
using UberSy.Domain.Payments.Enums;
using Xunit;

namespace UberSy.Domain.UnitTests.Payments;

public class PaymentTests
{
    // ============================================================
    // Helper Methods
    // ============================================================
    private static TbPayment CreateValidPayment()
    {
        return TbPayment.Create(
            id: Guid.NewGuid(),
            serviceRequestId: Guid.NewGuid(),
            amount: 100m,
            method: PaymentMethod.Cash,
            transactionId: "TXN-123456"
        ).Value;
    }

    // ============================================================
    // Create Tests
    // ============================================================

    [Fact]
    public void Create_ShouldReturnError_WhenServiceRequestIdIsEmpty()
    {
        var result = TbPayment.Create(
            id: Guid.NewGuid(),
            serviceRequestId: Guid.Empty,
            amount: 100m,
            method: PaymentMethod.Cash,
            transactionId: null);

        Assert.False(result.IsSuccess);
        Assert.Equal(PaymentErrors.ServiceRequestIdRequired.Code, result.TopError.Code);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Create_ShouldReturnError_WhenAmountIsZeroOrNegative(decimal invalidAmount)
    {
        var result = TbPayment.Create(
            id: Guid.NewGuid(),
            serviceRequestId: Guid.NewGuid(),
            amount: invalidAmount,
            method: PaymentMethod.Cash,
            transactionId: null);

        Assert.False(result.IsSuccess);
        Assert.Equal(PaymentErrors.AmountZero.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenTransactionIdIsTooLong()
    {
        var result = TbPayment.Create(
            id: Guid.NewGuid(),
            serviceRequestId: Guid.NewGuid(),
            amount: 100m,
            method: PaymentMethod.Cash,
            transactionId: new string('A', 101)); // 101 characters

        Assert.False(result.IsSuccess);
        Assert.Equal(PaymentErrors.TransactionIdTooLong.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_AndSetStatusToPending()
    {
        // Arrange
        var id = Guid.NewGuid();
        var serviceRequestId = Guid.NewGuid();
        var amount = 150.50m;
        var transactionId = "  TXN-TRIM  ";

        // Act
        var result = TbPayment.Create(id, serviceRequestId, amount, PaymentMethod.Electronic, transactionId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(id, result.Value.Id);
        Assert.Equal(serviceRequestId, result.Value.ServiceRequestId);
        Assert.Equal(amount, result.Value.Amount);
        Assert.Equal(PaymentMethod.Electronic, result.Value.Method);
        Assert.Equal("TXN-TRIM", result.Value.TransactionId); // Verify Trim()

        // Verify Defaults
        Assert.Equal(PaymentStatus.Pending, result.Value.Status);
        Assert.Null(result.Value.PaidAt);
    }

    // ============================================================
    // MarkAsCompleted Tests
    // ============================================================

    [Fact]
    public void MarkAsCompleted_ShouldReturnSuccess_AndSetStatusToCompleted()
    {
        var payment = CreateValidPayment();
        Assert.Equal(PaymentStatus.Pending, payment.Status);

        var result = payment.MarkAsCompleted();

        Assert.True(result.IsSuccess);
        Assert.Equal(PaymentStatus.Completed, payment.Status);
        Assert.NotNull(payment.PaidAt); // Verify PaidAt is set
    }

    [Fact]
    public void MarkAsCompleted_ShouldReturnError_WhenAlreadyCompleted()
    {
        var payment = CreateValidPayment();
        payment.MarkAsCompleted(); // Mark completed first

        var result = payment.MarkAsCompleted(); // Try again

        Assert.False(result.IsSuccess);
        Assert.Equal(PaymentErrors.PaymentAlreadyCompleted.Code, result.TopError.Code);
    }

    // ============================================================
    // MarkAsFailed Tests
    // ============================================================

    [Fact]
    public void MarkAsFailed_ShouldReturnSuccess_AndSetStatusToFailed()
    {
        var payment = CreateValidPayment();

        var result = payment.MarkAsFailed();

        Assert.True(result.IsSuccess);
        Assert.Equal(PaymentStatus.Failed, payment.Status);
        Assert.Null(payment.PaidAt); // Verify PaidAt remains null
    }

    [Fact]
    public void MarkAsFailed_ShouldReturnError_WhenAlreadyFailed()
    {
        var payment = CreateValidPayment();
        payment.MarkAsFailed(); // Mark failed first

        var result = payment.MarkAsFailed(); // Try again

        Assert.False(result.IsSuccess);
        Assert.Equal(PaymentErrors.PaymentAlreadyFailed.Code, result.TopError.Code);
    }
}
