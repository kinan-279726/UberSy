using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Helpers;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Admin.Dtos;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Drivers.Enums;
using UberSy.Domain.Identity;
using UberSy.Domain.ServiceRequests.Enums;

namespace UberSy.Application.Features.Admin.Queries.AdminDashboard;

public sealed class GetAdminDashboardQueryHandler(IApplicationDbContext context, IUser user)
    : IRequestHandler<GetAdminDashboardQuery, Result<AdminDashboardDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IUser _user = user;

    public async Task<Result<AdminDashboardDto>> Handle(
        GetAdminDashboardQuery request, CancellationToken cancellationToken)
    {
        if (!_user.IsInRole(Roles.Admin)) return ApplicationErrors.AccessDenied;

        var from = request.From ?? DamascusTime.Today();
        var to = request.To ?? from;
        var fromUtc = DamascusTime.StartOfDayUtc(from);
        var toUtc = DamascusTime.StartOfDayUtc(to.AddDays(1));

        var byStatus = await _context.TbFoodOrders
            .AsNoTracking()
            .Where(o => o.IdNavigation.CreatedAtUtc >= fromUtc && o.IdNavigation.CreatedAtUtc < toUtc)
            .GroupBy(o => o.IdNavigation.Status)
            .Select(g => new
            {
                Status = g.Key,
                Count = g.Count(),
                Sales = g.Sum(x => x.IdNavigation.TotalAmount),
                Revenue = g.Sum(x => x.PlatformCommissionFromRestaurant + (x.DeliveryFee - x.DriverEarning)),
            })
            .ToListAsync(cancellationToken);

        int CountOf(ServiceRequestStatus s) => byStatus.Where(x => x.Status == s).Sum(x => x.Count);
        var delivered = byStatus.FirstOrDefault(x => x.Status == ServiceRequestStatus.Delivered);

        // أرقام لحظية لا تعتمد على الفترة
        var waiting = await _context.TbFoodOrders.AsNoTracking()
            .CountAsync(o => o.DriverId == null
                             && o.IdNavigation.Status == ServiceRequestStatus.Preparing, cancellationToken);

        var online = await _context.TbDrivers.AsNoTracking()
            .CountAsync(d => d.Status == DriverStatus.Online, cancellationToken);

        var onDelivery = await _context.TbDrivers.AsNoTracking()
            .CountAsync(d => d.Status == DriverStatus.OnDelivery, cancellationToken);

        var noBalance = await _context.TbDrivers.AsNoTracking()
            .CountAsync(d => d.IsApproved && d.DriverWallet != null && d.DriverWallet.Balance <= 0, cancellationToken);

        var pendingRestaurants = await _context.TbRestaurants.AsNoTracking()
            .CountAsync(r => !r.IsApproved, cancellationToken);

        var pendingDrivers = await _context.TbDrivers.AsNoTracking()
            .CountAsync(d => !d.IsApproved, cancellationToken);

        return new AdminDashboardDto
        {
            From = from,
            To = to,
            TotalOrders = byStatus.Sum(x => x.Count),
            PendingOrders = CountOf(ServiceRequestStatus.Pending),
            PreparingOrders = CountOf(ServiceRequestStatus.Preparing),
            OutForDeliveryOrders = CountOf(ServiceRequestStatus.OutForDelivery),
            DeliveredOrders = CountOf(ServiceRequestStatus.Delivered),
            CancelledOrders = CountOf(ServiceRequestStatus.Cancelled),
            GrossSales = delivered?.Sales ?? 0m,
            PlatformRevenue = delivered?.Revenue ?? 0m,
            OrdersWaitingForDriver = waiting,
            DriversOnline = online,
            DriversOnDelivery = onDelivery,
            DriversWithoutBalance = noBalance,
            RestaurantsPendingApproval = pendingRestaurants,
            DriversPendingApproval = pendingDrivers,
        };
    }
}