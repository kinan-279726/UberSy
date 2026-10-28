using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.DeliveryRule.Enums;

namespace UberSy.Application.Features.DeliveryPricingRules;

public static class PricingRuleScheduleChecker
{
    public static async Task<bool> HasOverlapAsync(
        IApplicationDbContext context,
        VehicleType vehicleType,
        ServiceTier serviceTier,
        CarType? carType,
        TimeOnly? startHour,
        TimeOnly? endHour,
        Guid? excludeRuleId,
        CancellationToken cancellationToken)
    {
        var others = await context.TbDeliveryPricingRules
            .AsNoTracking()
            .Where(r => r.IsActive
                && !r.IsDeleted
                && r.VehicleType == vehicleType
                && r.ServiceTier == serviceTier
                && r.CarType == carType
                && (excludeRuleId == null || r.Id != excludeRuleId))
            .Select(r => new { r.StartHour, r.EndHour })
            .ToListAsync(cancellationToken);

        var start = startHour ?? TimeOnly.MinValue;
        var end = endHour ?? TimeOnly.MaxValue;

        return others.Any(o =>
            start < (o.EndHour ?? TimeOnly.MaxValue) &&
            (o.StartHour ?? TimeOnly.MinValue) < end);
    }
}