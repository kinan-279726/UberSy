using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Admin.Dtos;
using UberSy.Application.Features.ServiceRequests.Dtos;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Identity;

namespace UberSy.Application.Features.Admin.Queries.OrderDetails;

public sealed class GetAdminOrderDetailsQueryHandler(
    IApplicationDbContext context,
    IIdentityService identityService,
    IUser user)
    : IRequestHandler<GetAdminOrderDetailsQuery, Result<AdminOrderDetailsDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IIdentityService _identityService = identityService;
    private readonly IUser _user = user;

    public async Task<Result<AdminOrderDetailsDto>> Handle(
        GetAdminOrderDetailsQuery request, CancellationToken cancellationToken)
    {
        if (!_user.IsInRole(Roles.Admin)) return ApplicationErrors.AccessDenied;

        var order = await _context.TbFoodOrders
            .AsNoTracking()
            .Include(o => o.IdNavigation)
            .Include(o => o.Restaurant)
            .Include(o => o.Address)
            .Include(o => o.Driver)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Item)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.AddOns).ThenInclude(a => a.AddOn)
            .FirstOrDefaultAsync(o => o.Id == request.ServiceRequestId, cancellationToken);

        if (order is null) return ApplicationErrors.ServiceRequestNotFound;

        var history = await _context.TbServiceRequestHistories
            .AsNoTracking()
            .Where(h => h.ServiceRequestId == order.Id)
            .OrderBy(h => h.CreatedAtUtc)
            .Select(h => new AdminOrderHistoryDto { Status = h.Status, Note = h.Note, CreatedAtUtc = h.CreatedAtUtc })
            .ToListAsync(cancellationToken);

        var offers = await _context.TbServiceRequestOffers
            .AsNoTracking()
            .Where(o => o.ServiceRequestId == order.Id)
            .OrderBy(o => o.SentAt)
            .Select(o => new AdminOrderOfferDto
            {
                OfferId = o.Id,
                DriverId = o.DriverId,
                Status = o.Status,
                SentAt = o.SentAt,
                RespondedAt = o.RespondedAt,
            })
            .ToListAsync(cancellationToken);

        var customerEmail = await _identityService.GetUserNameAsync(order.IdNavigation.CustomerId);

        var itemsTotal = order.OrderItems
            .Sum(oi => (oi.UnitPrice + oi.AddOns.Sum(a => a.Price)) * oi.Quantity);

        return new AdminOrderDetailsDto
        {
            ServiceRequestId = order.Id,
            TrackingNumber = order.IdNavigation.TrackingNumber,
            Status = order.IdNavigation.Status,
            CancellationReason = order.IdNavigation.CancellationReason,
            CreatedAtUtc = order.IdNavigation.CreatedAtUtc,
            Restaurant = new AdminOrderRestaurantDto
            {
                Id = order.RestaurantId,
                NameAr = order.Restaurant.NameAr,
                Phone = order.Restaurant.Phone,
            },
            Customer = new AdminOrderCustomerDto
            {
                UserId = order.IdNavigation.CustomerId,
                Email = customerEmail,
                AddressLabel = order.Address.Label ?? order.Address.Street,
                Street = order.Address.Street,
                City = order.Address.City,
                Floor = order.Address.Floor,
                Phone = order.Address.Phone,
                Latitude = order.Address.Boundary.Y,
                Longitude = order.Address.Boundary.X,
            },
            Driver = order.Driver is null ? null : new AdminOrderDriverDto
            {
                DriverId = order.Driver.Id,
                UserId = order.Driver.UserId,
                VehicleType = order.Driver.VehicleType,
                PlateNumber = order.Driver.PlateNumber,
                Status = order.Driver.Status,
            },
            Items = order.OrderItems.Select(oi => new FoodOrderLineDto
            {
                ItemNameAr = oi.Item.NameAr,
                ItemNameEn = oi.Item.NameEn,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice,
                Note = oi.Note,
                AddOnNamesAr = oi.AddOns.Select(a => a.AddOn.NameAr).ToList(),
            }).ToList(),
            ItemsTotal = itemsTotal,
            DeliveryFee = order.DeliveryFee,
            DriverEarning = order.DriverEarning,
            RestaurantCommission = order.PlatformCommissionFromRestaurant,
            PlatformRevenue = order.PlatformCommissionFromRestaurant + (order.DeliveryFee - order.DriverEarning),
            CashToCollect = order.IdNavigation.TotalAmount + order.DeliveryFee,
            History = history,
            Offers = offers,
        };
    }
}