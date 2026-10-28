using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Drivers.Dtos;
using UberSy.Domain.Common.Results;
using UberSy.Domain.ServiceRequests.Offers.Enums;

namespace UberSy.Application.Features.Drivers.Queries.MyPendingOffers;

public sealed class GetMyPendingOffersQueryHandler(IApplicationDbContext context, IOwnershipValidator ownership)
    : IRequestHandler<GetMyPendingOffersQuery, Result<List<DriverOfferSummaryDto>>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IOwnershipValidator _ownership = ownership;

    public async Task<Result<List<DriverOfferSummaryDto>>> Handle(
        GetMyPendingOffersQuery request, CancellationToken cancellationToken)
    {
        var driverResult = await _ownership.IsDriver();
        if (driverResult.IsError) return driverResult.Errors;
        var driverId = driverResult.Value;

        var now = DateTimeOffset.UtcNow;

        var offers = await (
            from offer in _context.TbServiceRequestOffers.AsNoTracking()
            join order in _context.TbFoodOrders.AsNoTracking() on offer.ServiceRequestId equals order.Id
            where offer.DriverId == driverId
                  && offer.Status == OfferStatus.Sent
                  && offer.ResponseTimeout > now
            orderby offer.ResponseTimeout
            select new
            {
                offer.Id,
                offer.ServiceRequestId,
                order.IdNavigation.TrackingNumber,
                RestaurantNameAr = order.Restaurant.NameAr,
                offer.ResponseTimeout,
            })
            .ToListAsync(cancellationToken);

        return offers.Select(o => new DriverOfferSummaryDto
        {
            OfferId = o.Id,
            ServiceRequestId = o.ServiceRequestId,
            TrackingNumber = o.TrackingNumber,
            RestaurantNameAr = o.RestaurantNameAr,
            ExpiresAtUtc = o.ResponseTimeout,
            SecondsRemaining = (int)Math.Max(0, (o.ResponseTimeout - now).TotalSeconds),
        }).ToList();
    }
}