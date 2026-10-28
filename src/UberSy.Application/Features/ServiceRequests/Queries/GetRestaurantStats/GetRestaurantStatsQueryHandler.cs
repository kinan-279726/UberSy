using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Helpers;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.ServiceRequests.Dtos;
using UberSy.Domain.Common.Results;
using UberSy.Domain.ServiceRequests.Enums;

namespace UberSy.Application.Features.ServiceRequests.Queries.GetRestaurantStats;

public sealed class GetRestaurantStatsQueryHandler(IApplicationDbContext context, IOwnershipValidator ownership)
    : IRequestHandler<GetRestaurantStatsQuery, Result<RestaurantStatsDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IOwnershipValidator _ownership = ownership;

    public async Task<Result<RestaurantStatsDto>> Handle(GetRestaurantStatsQuery request, CancellationToken cancellationToken)
    {
        var ownerResult = await _ownership.EnsureIsOwnerByRestaurantIdAsync(request.RestaurantId, cancellationToken);
        if (ownerResult.IsError) return ownerResult.Errors;

        var from = request.From ?? DamascusTime.Today();
        var to = request.To ?? from;
        var fromUtc = DamascusTime.StartOfDayUtc(from);
        var toUtc = DamascusTime.StartOfDayUtc(to.AddDays(1));

        var orders = _context.TbFoodOrders.AsNoTracking()
            .Where(o => o.RestaurantId == request.RestaurantId
                && o.IdNavigation.CreatedAtUtc >= fromUtc
                && o.IdNavigation.CreatedAtUtc < toUtc);

        var byStatus = await orders
            .GroupBy(o => o.IdNavigation.Status)
            .Select(g => new
            {
                Status = g.Key,
                Count = g.Count(),
                Sales = g.Sum(x => x.IdNavigation.TotalAmount),
                Commission = g.Sum(x => x.PlatformCommissionFromRestaurant)
            })
            .ToListAsync(cancellationToken);

        var delivered = byStatus.FirstOrDefault(x => x.Status == ServiceRequestStatus.Delivered);
        var deliveredCount = delivered?.Count ?? 0;
        var gross = delivered?.Sales ?? 0m;
        var commission = delivered?.Commission ?? 0m;

        var dailyRaw = await orders
            .Where(o => o.IdNavigation.Status == ServiceRequestStatus.Delivered)
            .Select(o => new { o.IdNavigation.CreatedAtUtc, o.IdNavigation.TotalAmount, o.PlatformCommissionFromRestaurant })
            .ToListAsync(cancellationToken);

        var daily = dailyRaw
            .GroupBy(x => DateOnly.FromDateTime(x.CreatedAtUtc.ToOffset(TimeSpan.FromHours(3)).DateTime))
            .OrderBy(g => g.Key)
            .Select(g => new DailyStatDto(g.Key, g.Count(),
                g.Sum(x => x.TotalAmount),
                g.Sum(x => x.TotalAmount - x.PlatformCommissionFromRestaurant)))
            .ToList();

        var topItems = await _context.TbOrderItems.AsNoTracking()
            .Where(oi => oi.FoodOrder.RestaurantId == request.RestaurantId
                && oi.FoodOrder.IdNavigation.Status == ServiceRequestStatus.Delivered
                && oi.FoodOrder.IdNavigation.CreatedAtUtc >= fromUtc
                && oi.FoodOrder.IdNavigation.CreatedAtUtc < toUtc)
            .GroupBy(oi => new { oi.ItemId, oi.Item.NameAr, oi.Item.NameEn })
            .Select(g => new TopItemDto(g.Key.ItemId, g.Key.NameAr, g.Key.NameEn, g.Sum(x => x.Quantity)))
            .OrderByDescending(x => x.QuantitySold)
            .Take(5)
            .ToListAsync(cancellationToken);

        return new RestaurantStatsDto(
            from, to,
            byStatus.Sum(x => x.Count),
            deliveredCount,
            byStatus.Where(x => x.Status == ServiceRequestStatus.Cancelled).Sum(x => x.Count),
            byStatus.Where(x => x.Status is ServiceRequestStatus.Pending or ServiceRequestStatus.Preparing
                                        or ServiceRequestStatus.OutForDelivery).Sum(x => x.Count),
            gross, commission, gross - commission,
            deliveredCount == 0 ? 0 : Math.Round(gross / deliveredCount, 2),
            daily, topItems);
    }
}