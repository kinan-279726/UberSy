using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Drivers.Dtos;
using UberSy.Application.Features.ServiceRequests.Offers.Dtos;
using UberSy.Domain.Common.Results;
using UberSy.Domain.ServiceRequests.Enums;

namespace UberSy.Application.Features.Drivers.Queries.MyActiveOrder;

public sealed class GetMyActiveOrderQueryHandler(IApplicationDbContext context, IOwnershipValidator ownership)
    : IRequestHandler<GetMyActiveOrderQuery, Result<ActiveDeliveryDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IOwnershipValidator _ownership = ownership;

    public async Task<Result<ActiveDeliveryDto>> Handle(
        GetMyActiveOrderQuery request, CancellationToken cancellationToken)
    {
        var driverResult = await _ownership.IsDriver();
        if (driverResult.IsError) return driverResult.Errors;
        var driverId = driverResult.Value;

        var order = await _context.TbFoodOrders
            .AsNoTracking()
            .Include(o => o.IdNavigation)
            .Include(o => o.Restaurant)
            .Include(o => o.Address)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Item)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.AddOns).ThenInclude(a => a.AddOn)
            .Where(o => o.DriverId == driverId
                        && (o.IdNavigation.Status == ServiceRequestStatus.Preparing
                            || o.IdNavigation.Status == ServiceRequestStatus.OutForDelivery))
            .FirstOrDefaultAsync(cancellationToken);

        if (order is null) return ApplicationErrors.ServiceRequestNotFound;

        var itemsSubtotal = order.OrderItems
            .Sum(oi => (oi.UnitPrice + oi.AddOns.Sum(a => a.Price)) * oi.Quantity);

        return new ActiveDeliveryDto
        {
            ServiceRequestId = order.Id,
            TrackingNumber = order.IdNavigation.TrackingNumber,
            Status = order.IdNavigation.Status,
            Restaurant = new OfferPointDto
            {
                NameAr = order.Restaurant.NameAr,
                NameEn = order.Restaurant.NameEn,
                Latitude = order.Restaurant.Boundary.Y,
                Longitude = order.Restaurant.Boundary.X,
            },
            RestaurantPhone = order.Restaurant.Phone,
            Customer = new OfferPointDto
            {
                NameAr = order.Address.Label ?? order.Address.Street,
                Street = order.Address.Street,
                City = order.Address.City,
                Floor = order.Address.Floor,
                Latitude = order.Address.Boundary.Y,
                Longitude = order.Address.Boundary.X,
            },
            CustomerPhone = order.Address.Phone,
            Items = order.OrderItems.Select(oi => new OfferItemDto
            {
                NameAr = oi.Item.NameAr,
                NameEn = oi.Item.NameEn,
                Quantity = oi.Quantity,
                Note = oi.Note,
                AddOnNamesAr = oi.AddOns.Select(a => a.AddOn.NameAr).ToList(),
            }).ToList(),
            RestaurantPrice = itemsSubtotal - order.PlatformCommissionFromRestaurant,
            DeliveryFee = order.DeliveryFee,
            CashToCollect = order.IdNavigation.TotalAmount + order.DeliveryFee,
            DriverEarning = order.DriverEarning,
        };
    }
}