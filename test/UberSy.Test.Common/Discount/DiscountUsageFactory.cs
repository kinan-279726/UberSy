using UberSy.Domain.Common.Results;
using UberSy.Domain.Discounts;

namespace UberSy.Test.Common.Discount;

public static class DiscountUsageFactory
{
    public static Result<TbDiscountUsage> CreateDiscountUsage(
        Guid? id = null,
        Guid? discountId = null,
        Guid? customerId = null,
        Guid? serviceRequestId = null)
    {
        return TbDiscountUsage.Create(
            id ?? Guid.NewGuid(),
            discountId ?? Guid.NewGuid(),
            customerId ?? Guid.NewGuid(),
            serviceRequestId ?? Guid.NewGuid());
    }
}
