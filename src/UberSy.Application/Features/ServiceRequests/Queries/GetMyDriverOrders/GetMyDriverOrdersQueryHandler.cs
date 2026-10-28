using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Common.Models;
using UberSy.Application.Features.ServiceRequests.Dtos;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.ServiceRequests.Queries.GetMyDriverOrders;

public sealed class GetMyDriverOrdersQueryHandler(IApplicationDbContext context, IOwnershipValidator ownership)
    : IRequestHandler<GetMyDriverOrdersQuery, Result<PagedList<DriverOrderListItemDto>>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IOwnershipValidator _ownership = ownership;

    public async Task<Result<PagedList<DriverOrderListItemDto>>> Handle(
        GetMyDriverOrdersQuery request, CancellationToken cancellationToken)
    {
        var driverResult = await _ownership.IsDriver();
        if (driverResult.IsError) return driverResult.Errors;
        var driverId = driverResult.Value;

        var query = _context.TbFoodOrders.AsNoTracking().Where(o => o.DriverId == driverId);

        if (request.Status.HasValue)
            query = query.Where(o => o.IdNavigation.Status == request.Status.Value);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(o => o.IdNavigation.CreatedAtUtc)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(o => new DriverOrderListItemDto(
                o.Id, o.IdNavigation.TrackingNumber, o.IdNavigation.Status,
                o.Restaurant.NameAr, o.Address.Street, o.Address.City,
                o.IdNavigation.TotalAmount + o.DeliveryFee, o.DeliveryFee, o.DriverEarning,
                o.IdNavigation.CreatedAtUtc))
            .ToListAsync(cancellationToken);

        return PagedList<DriverOrderListItemDto>.Create(items, request.PageNumber, request.PageSize, total);
    }
}