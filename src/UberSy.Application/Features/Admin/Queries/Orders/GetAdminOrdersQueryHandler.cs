using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Helpers;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Common.Models;
using UberSy.Application.Features.Admin.Dtos;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Identity;
using UberSy.Domain.ServiceRequests.Enums;

namespace UberSy.Application.Features.Admin.Queries.Orders;

public sealed class GetAdminOrdersQueryHandler(IApplicationDbContext context, IUser user)
    : IRequestHandler<GetAdminOrdersQuery, Result<PagedList<AdminOrderListItemDto>>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IUser _user = user;

    public async Task<Result<PagedList<AdminOrderListItemDto>>> Handle(
        GetAdminOrdersQuery request, CancellationToken cancellationToken)
    {
        if (!_user.IsInRole(Roles.Admin)) return ApplicationErrors.AccessDenied;

        var query = _context.TbFoodOrders.AsNoTracking().AsQueryable();

        if (request.Status.HasValue)
            query = query.Where(o => o.IdNavigation.Status == request.Status.Value);

        if (request.RestaurantId.HasValue)
            query = query.Where(o => o.RestaurantId == request.RestaurantId.Value);

        if (request.DriverId.HasValue)
            query = query.Where(o => o.DriverId == request.DriverId.Value);

        if (request.TrackingNumber.HasValue)
            query = query.Where(o => o.IdNavigation.TrackingNumber == request.TrackingNumber.Value);

        if (request.WaitingForDriver == true)
            query = query.Where(o => o.DriverId == null
                                     && o.IdNavigation.Status == ServiceRequestStatus.Preparing);

        if (request.From.HasValue)
        {
            var fromUtc = DamascusTime.StartOfDayUtc(request.From.Value);
            query = query.Where(o => o.IdNavigation.CreatedAtUtc >= fromUtc);
        }

        if (request.To.HasValue)
        {
            var toUtc = DamascusTime.StartOfDayUtc(request.To.Value.AddDays(1));
            query = query.Where(o => o.IdNavigation.CreatedAtUtc < toUtc);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(o => o.IdNavigation.CreatedAtUtc)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(o => new AdminOrderListItemDto
            {
                ServiceRequestId = o.Id,
                TrackingNumber = o.IdNavigation.TrackingNumber,
                Status = o.IdNavigation.Status,
                CustomerId = o.IdNavigation.CustomerId,
                RestaurantId = o.RestaurantId,
                RestaurantNameAr = o.Restaurant.NameAr,
                DriverId = o.DriverId,
                WaitingForDriver = o.DriverId == null
                                   && o.IdNavigation.Status == ServiceRequestStatus.Preparing,
                TotalAmount = o.IdNavigation.TotalAmount,
                DeliveryFee = o.DeliveryFee,
                CreatedAtUtc = o.IdNavigation.CreatedAtUtc,
            })
            .ToListAsync(cancellationToken);

        return PagedList<AdminOrderListItemDto>.Create(items, request.PageNumber, request.PageSize, totalCount);
    }
}