using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.ServiceRequests.Dtos;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Identity;

namespace UberSy.Application.Features.ServiceRequests.Queries.GetFoodOrderDetails;

public sealed class GetFoodOrderDetailsQueryHandler(IApplicationDbContext context, IUser user)
    : IRequestHandler<GetFoodOrderDetailsQuery, Result<FoodOrderDetailsDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IUser _user = user;

    public async Task<Result<FoodOrderDetailsDto>> Handle(GetFoodOrderDetailsQuery query, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_user.Id) || !Guid.TryParse(_user.Id, out var userId))
            return ApplicationErrors.Unauthorized;

        var foodOrder = await _context.TbFoodOrders
            .AsNoTracking()
            .Include(o => o.Restaurant)
            .Include(o => o.IdNavigation)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Item)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.AddOns).ThenInclude(a => a.AddOn)
            .FirstOrDefaultAsync(o => o.Id == query.ServiceRequestId, ct);

        if (foodOrder is null) return ApplicationErrors.ServiceRequestNotFound;

        // Allowed: the customer, the restaurant owner, the assigned driver, or an admin
        var isAllowed = _user.IsInRole(Roles.Admin)
            || foodOrder.IdNavigation.CustomerId == userId
            || foodOrder.Restaurant.OwnerId == userId
            || foodOrder.DriverId == userId;

        if (!isAllowed) return ApplicationErrors.AccessDenied;

        return new FoodOrderDetailsDto
        {
            ServiceRequestId = foodOrder.Id,
            TrackingNumber = foodOrder.IdNavigation.TrackingNumber,
            Status = (byte)foodOrder.IdNavigation.Status,
            TotalAmount = foodOrder.IdNavigation.TotalAmount,
            ResponseDeadlineUtc = foodOrder.ResponseTimeout,
            Items = foodOrder.OrderItems.Select(oi => new FoodOrderLineDto
            {
                ItemNameAr = oi.Item.NameAr,
                ItemNameEn = oi.Item.NameEn,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice,
                Note = oi.Note,
                AddOnNamesAr = oi.AddOns.Select(a => a.AddOn.NameAr).ToList(),
            }).ToList(),
        };
    }
}
