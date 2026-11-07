using UberSy.Domain.Common.Results;
using UberSy.Domain.Discounts;
using UberSy.Domain.Discounts.Enums;

namespace UberSy.Test.Common.Discount;

public static class DiscountFactory
{
    public static Result<TbDiscount> CreateDiscount(
        Guid? id = null,
        Guid? restaurantId = null,
        string? code = null,
        DiscountType? type = null,
        decimal? value = null,
        DiscountScope? scope = null,
        decimal? minOrderAmount = null,
        int? maxUsageCount = null,
        byte? maxUsagePerCustomer = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        return TbDiscount.Create(
            id ?? Guid.NewGuid(),
            restaurantId,
            code ?? "WELCOME10",
            type ?? DiscountType.Percentage,
            value ?? 10m,
            scope ?? DiscountScope.Order,
            minOrderAmount,
            maxUsageCount,
            maxUsagePerCustomer,
            startDate ?? DateTime.UtcNow,
            endDate ?? DateTime.UtcNow.AddDays(30));
    }
}
