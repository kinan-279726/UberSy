using UberSy.Domain.Common;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Complaints;
using UberSy.Domain.Discounts;
using UberSy.Domain.Drivers.Wallet;
using UberSy.Domain.Payments;
using UberSy.Domain.Reservations;
using UberSy.Domain.Reviews;
using UberSy.Domain.ServiceRequests.Enums;
using UberSy.Domain.ServiceRequests.Events.LifecycleEvents;
using UberSy.Domain.ServiceRequests.FoodOrders;
using UberSy.Domain.ServiceRequests.History;
using UberSy.Domain.ServiceRequests.Offers;
using UberSy.Domain.ServiceRequests.RideRequests;

namespace UberSy.Domain.ServiceRequests;

public sealed class TbServiceRequest : AuditableEntity
{
    public Guid CustomerId { get; private set; }

    public ServiceType ServiceType { get; private set; }

    public int TrackingNumber { get; private set; }

    public ServiceRequestStatus Status { get; private set; }

    public decimal TotalAmount { get; private set; }

    public string? CancellationReason { get; private set; }

    public TbFoodOrder? FoodOrder { get; private set; }

    public TbTableReservation? TableReservation { get; private set; }

    public TbRideRequest? RideRequest { get; private set; }

    private readonly List<TbComplaint> _complaints = [];
    public IReadOnlyCollection<TbComplaint> Complaints => _complaints.AsReadOnly();

    private readonly List<TbDiscountUsage> _discountUsages = [];
    public IReadOnlyCollection<TbDiscountUsage> DiscountUsages => _discountUsages.AsReadOnly();

    private readonly List<TbDriverWalletTransaction> _walletTransactions = [];
    public IReadOnlyCollection<TbDriverWalletTransaction> WalletTransactions => _walletTransactions.AsReadOnly();

    private readonly List<TbPayment> _payments = [];
    public IReadOnlyCollection<TbPayment> Payments => _payments.AsReadOnly();

    private readonly List<TbReview> _reviews = [];
    public IReadOnlyCollection<TbReview> Reviews => _reviews.AsReadOnly();

    private readonly List<TbServiceRequestHistory> _history = [];
    public IReadOnlyCollection<TbServiceRequestHistory> History => _history.AsReadOnly();

    private readonly List<TbServiceRequestOffer> _offers = [];
    public IReadOnlyCollection<TbServiceRequestOffer> Offers => _offers.AsReadOnly();

    private TbServiceRequest()
    { }

    private TbServiceRequest(Guid id, Guid customerId, ServiceType serviceType, decimal totalAmount)
        : base(id)
    {
        CustomerId = customerId;
        ServiceType = serviceType;
        TotalAmount = totalAmount;
        Status = ServiceRequestStatus.Pending;
    }

    public static Result<TbServiceRequest> Create(Guid id, Guid customerId, ServiceType serviceType, decimal totalAmount)
    {
        if (customerId == Guid.Empty) return ServiceRequestErrors.CustomerIdRequired;
        if (!Enum.IsDefined(serviceType)) return ServiceRequestErrors.ServiceTypeInvalid;
        if (totalAmount <= 0) return ServiceRequestErrors.TotalAmountZero;

        return new TbServiceRequest(id, customerId, serviceType, totalAmount);
    }

    public Result<Updated> UpdateTotalAmount(decimal totalAmount)
    {
        if (Status is ServiceRequestStatus.Delivered or ServiceRequestStatus.Cancelled)
            return ServiceRequestErrors.CannotModifyCompletedRequest;

        if (totalAmount <= 0) return ServiceRequestErrors.TotalAmountZero;

        TotalAmount = totalAmount;

        return Result.Updated;
    }

    public Result<Updated> MarkAsPreparing()
    {
        var check = EnsureTransition(ServiceRequestStatus.Preparing);
        if (check.IsError) return check.Errors;

        Status = ServiceRequestStatus.Preparing;

        AddDomainEvent(new ServiceRequestPreparing(Id, CustomerId));

        return Result.Updated;
    }

    public Result<Updated> MarkAsOutForDelivery()
    {
        var check = EnsureTransition(ServiceRequestStatus.OutForDelivery);
        if (check.IsError) return check.Errors;

        Status = ServiceRequestStatus.OutForDelivery;

        AddDomainEvent(new ServiceRequestOutForDelivery(Id, CustomerId));

        return Result.Updated;
    }

    public Result<Updated> MarkAsDelivered()
    {
        var check = EnsureTransition(ServiceRequestStatus.Delivered);
        if (check.IsError) return check.Errors;

        Status = ServiceRequestStatus.Delivered;

        AddDomainEvent(new ServiceRequestDelivered(Id, CustomerId));

        return Result.Updated;
    }

    public Result<Updated> Cancel(string reason)
    {
        var check = EnsureTransition(ServiceRequestStatus.Cancelled);
        if (check.IsError) return check.Errors;

        if (string.IsNullOrWhiteSpace(reason)) return ServiceRequestErrors.CancellationReasonRequired;
        if (reason.Length > 100) return ServiceRequestErrors.CancellationReasonTooLong;

        Status = ServiceRequestStatus.Cancelled;
        CancellationReason = reason.Trim();

        AddDomainEvent(new ServiceRequestCancelled(Id));

        return Result.Updated;
    }

    private Result<Success> EnsureTransition(ServiceRequestStatus newStatus)
    {
        var allowed = Status switch
        {
            ServiceRequestStatus.Pending => newStatus is ServiceRequestStatus.Preparing or ServiceRequestStatus.Cancelled,
            ServiceRequestStatus.Preparing => newStatus is ServiceRequestStatus.OutForDelivery or ServiceRequestStatus.Cancelled,
            ServiceRequestStatus.OutForDelivery => newStatus is ServiceRequestStatus.Delivered or ServiceRequestStatus.Cancelled,
            _ => false,
        };

        if (!allowed)
        {
            if (Status == ServiceRequestStatus.Cancelled) return ServiceRequestErrors.ServiceRequestAlreadyCancelled;
            if (Status == ServiceRequestStatus.Delivered) return ServiceRequestErrors.ServiceRequestAlreadyDelivered;
            return ServiceRequestErrors.InvalidStatusTransition;
        }

        return Result.Success;
    }
}
