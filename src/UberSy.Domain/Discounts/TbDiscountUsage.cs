using UberSy.Domain.Common;
using UberSy.Domain.Common.Results;
using UberSy.Domain.ServiceRequests;

namespace UberSy.Domain.Discounts;

public sealed class TbDiscountUsage : AuditableEntity
{
    public Guid DiscountId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid ServiceRequestId { get; private set; }
    public DateTime UsedAt { get; private set; }

    public TbDiscount Discount { get; private set; } = null!;
    public TbServiceRequest ServiceRequest { get; private set; } = null!;

    private TbDiscountUsage()
    { }

    private TbDiscountUsage(Guid id, Guid discountId, Guid customerId, Guid serviceRequestId)
        : base(id)
    {
        DiscountId = discountId;
        CustomerId = customerId;
        ServiceRequestId = serviceRequestId;
        UsedAt = DateTime.UtcNow;
    }

    public static Result<TbDiscountUsage> Create(Guid id, Guid discountId, Guid customerId, Guid serviceRequestId)
    {
        if (discountId == Guid.Empty) return DiscountErrors.DescountrIdRequired;
        if (customerId == Guid.Empty) return DiscountErrors.CustomerIdRequired;
        if (serviceRequestId == Guid.Empty) return DiscountErrors.ServiceIdRequired;

        return new TbDiscountUsage(id, discountId, customerId, serviceRequestId);
    }
}