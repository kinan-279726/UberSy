using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.ServiceRequests.Offers.Dtos;
using UberSy.Domain.Common.Results;
using UberSy.Domain.ServiceRequests.Offers.Enums;

namespace UberSy.Application.Features.ServiceRequests.Queries.GetPendingOffer;

public sealed class GetPendingOfferQueryHandler(IApplicationDbContext context,
    IOwnershipValidator ownership,
    IDeliveryPricingService pricing)
    : IRequestHandler<GetPendingOfferQuery, Result<PendingOfferDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IOwnershipValidator _ownership = ownership;
    private readonly IDeliveryPricingService _pricing = pricing;

    public async Task<Result<PendingOfferDto>> Handle(GetPendingOfferQuery request, CancellationToken cancellationToken)
    {
        var driverResult = await _ownership.IsDriver();
        if (driverResult.IsError) return driverResult.Errors;
        var driverId = driverResult.Value;

        var offer = await _context.TbServiceRequestOffers
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == request.OfferId, cancellationToken);

        if (offer is null) return ApplicationErrors.OfferNotFound;
        if (offer.DriverId != driverId) return ApplicationErrors.AccessDenied;

        if (offer.Status != OfferStatus.Sent || offer.ResponseTimeout <= DateTime.UtcNow)
        {
            return ApplicationErrors.OfferNoLongerAvailable;
        }

        var foodOrder = await _context.TbFoodOrders
            .AsNoTracking()
            .Include(o => o.IdNavigation)
            .Include(o => o.Restaurant)
            .Include(o => o.Address)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Item)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.AddOns).ThenInclude(a => a.AddOn)
            .FirstOrDefaultAsync(o => o.Id == offer.ServiceRequestId,cancellationToken);

        if (foodOrder is null) return ApplicationErrors.ServiceRequestNotFound;

        var driver = await _context.TbDrivers.AsNoTracking().FirstAsync(d => d.Id == driverId, cancellationToken);

        var restaurant = foodOrder.Restaurant;
        var address = foodOrder.Address;

        var quoteResult = await _pricing.CalculateAsync(restaurant.Boundary, address.Boundary, driver.VehicleType, driver.CarType, cancellationToken);
        if (quoteResult.IsError) return quoteResult.Errors;
        var quote = quoteResult.Value;

        var itemsSubtotal = foodOrder.OrderItems.Sum(oi => (oi.UnitPrice + oi.AddOns.Sum(a => a.Price)) * oi.Quantity);
        var commission = Math.Round(itemsSubtotal * restaurant.CommissionRate / 100m, 2);

        return new PendingOfferDto
        {
            OfferId = offer.Id,
            ServiceRequestId = foodOrder.Id,
            TrackingNumber = foodOrder.IdNavigation.TrackingNumber,
            ExpiresAtUtc = offer.ResponseTimeout,
            SecondsRemaining = (int)Math.Max(0, (offer.ResponseTimeout - DateTime.UtcNow).TotalSeconds),

            Restaurant = new OfferPointDto
            {
                NameAr = restaurant.NameAr,
                NameEn = restaurant.NameEn,
                Latitude = restaurant.Boundary.Y,
                Longitude = restaurant.Boundary.X,
            },
            Customer = new OfferPointDto
            {
                NameAr = address.Label ?? address.Street,
                Street = address.Street,
                City = address.City,
                Floor = address.Floor,
                Latitude = address.Boundary.Y,
                Longitude = address.Boundary.X,
            },
            DistanceKm = quote.DistanceKm,

            Items = foodOrder.OrderItems.Select(oi => new OfferItemDto
            {
                NameAr = oi.Item.NameAr,
                NameEn = oi.Item.NameEn,
                Quantity = oi.Quantity,
                Note = oi.Note,
                AddOnNamesAr = oi.AddOns.Select(a => a.AddOn.NameAr).ToList(),
            }).ToList(),

            RestaurantPrice = itemsSubtotal - commission,
            DeliveryFee = quote.CustomerFee,
            CustomerPrice = foodOrder.IdNavigation.TotalAmount + quote.CustomerFee,
            DriverEarning = quote.DriverEarning,
        };
    }
}
