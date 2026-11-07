using UberSy.Domain.Common.Results;
using UberSy.Domain.Payments;
using UberSy.Domain.Payments.Enums;

namespace UberSy.Test.Common.Payment;

public static class PaymentFactory
{
    public static Result<TbPayment> CreatePayment(
        Guid? id = null,
        Guid? serviceRequestId = null,
        decimal? amount = null,
        PaymentMethod? method = null,
        string? transactionId = null)
    {
        return TbPayment.Create(
            id ?? Guid.NewGuid(),
            serviceRequestId ?? Guid.NewGuid(),
            amount ?? 50m,
            method ?? PaymentMethod.Cash,
            transactionId);
    }
}
