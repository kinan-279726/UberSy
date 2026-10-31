using UberSy.Domain.Common;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Payments.Enums;
using UberSy.Domain.ServiceRequests;

namespace UberSy.Domain.Payments;

public sealed class TbPayment : AuditableEntity
{
    public Guid ServiceRequestId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentMethod Method { get; private set; }
    public string? TransactionId { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTimeOffset? PaidAt { get; private set; }

    public TbServiceRequest ServiceRequest { get; private set; } = null!;

    private TbPayment()
    { }

    private TbPayment(Guid id, Guid serviceRequestId, decimal amount, PaymentMethod method, string? transactionId)
        : base(id)
    {
        ServiceRequestId = serviceRequestId;
        Amount = amount;
        Method = method;
        TransactionId = transactionId;
        Status = PaymentStatus.Pending;
    }

    public static Result<TbPayment> Create(Guid id, Guid serviceRequestId, decimal amount, PaymentMethod method, string? transactionId)
    {
        if (serviceRequestId == Guid.Empty) return PaymentErrors.ServiceRequestIdRequired;
        if (amount <= 0) return PaymentErrors.AmountZero;
        if (transactionId is { Length: > 100 }) return PaymentErrors.TransactionIdTooLong;

        return new TbPayment(id, serviceRequestId, amount, method, transactionId?.Trim());
    }

    public Result<Updated> MarkAsCompleted()
    {
        if (Status == PaymentStatus.Completed) return PaymentErrors.PaymentAlreadyCompleted;

        Status = PaymentStatus.Completed;
        PaidAt = DateTimeOffset.UtcNow;

        return Result.Updated;
    }

    public Result<Updated> MarkAsFailed()
    {
        if (Status == PaymentStatus.Failed) return PaymentErrors.PaymentAlreadyFailed;

        Status = PaymentStatus.Failed;

        return Result.Updated;
    }
}
