using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;
using UberSy.Domain.DeliveryRule.Enums;
using UberSy.Domain.Drivers.Enums;
using UberSy.Domain.ServiceRequests.Enums;
using UberSy.Domain.ServiceRequests.Offers;

namespace UberSy.Application.Common.Services;

public sealed class DriverDispatchService(
    IApplicationDbContext context,
    IDriverNotifier notifier,
    IConfiguration configuration,
    ILogger<DriverDispatchService> logger)
    : IDriverDispatchService
{
    private readonly IApplicationDbContext _context = context;
    private readonly IDriverNotifier _notifier = notifier;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<DriverDispatchService> _logger = logger;

    public async Task<Result<Updated>> DispatchNextBatchAsync(Guid serviceRequestId, CancellationToken cancellation = default)
    {
        var foodOrder = await _context.TbFoodOrders
            .Include(o => o.Restaurant)
            .FirstOrDefaultAsync(o => o.Id == serviceRequestId, cancellation);

        if (foodOrder is null)
        {
            return ApplicationErrors.ServiceRequestNotFound;
        }

        if (foodOrder.DriverId is not null)
        {
            return Result.Updated;
        }

        if (foodOrder.IdNavigation.Status != ServiceRequestStatus.Preparing)
        {
            return Result.Updated;
        }

        var alreadyOfferedDriverIds = await _context.TbServiceRequestOffers
            .Where(o => o.ServiceRequestId == serviceRequestId)
            .Select(o => o.DriverId)
            .Distinct()
            .ToListAsync(cancellation);

        var restaurantLocation = foodOrder.Restaurant.Boundary;

        var batchSize = _configuration.GetValue<int>("DispatchSettings:BatchSize");

        var candidateDriverIds = await (
            from d in _context.TbDrivers.AsNoTracking()
            join w in _context.TbDriverWallets on d.Id equals w.DriverId
            join loc in _context.TbDriverLocations on d.Id equals loc.DriverId
            where d.Status == DriverStatus.Online
               && (d.VehicleType == VehicleType.Motorcycle || d.VehicleType == VehicleType.Bicycle)
               && !alreadyOfferedDriverIds.Contains(d.Id)
               && w.Balance > 0
            select new
            {
                DriverId = d.Id,
                Distance = loc.Boundary.Distance(restaurantLocation)
            }
        )
        .OrderBy(x => x.Distance)
        .Take(batchSize)
        .Select(x => x.DriverId)
        .ToListAsync(cancellation);

        if (candidateDriverIds.Count == 0)
        {
            _logger.LogWarning("No Available Drivers Left For ServiceRequest '{Id}' (Already Offered To {Count}).",
                serviceRequestId, alreadyOfferedDriverIds.Count);

            return ApplicationErrors.NoAvailableDriverFound;
        }

        var offerTimeoutMinutes = _configuration.GetValue<int>("DispatchSettings:OfferTimeoutMinutes");
        var createdOffers = new List<TbServiceRequestOffer>();

        foreach (var driverId in candidateDriverIds)
        {
            var offerResult = TbServiceRequestOffer.Create(
                id: Guid.NewGuid(),
                serviceRequestId: serviceRequestId,
                driverId: driverId,
                responseTimeout: DateTimeOffset.UtcNow.AddMinutes(offerTimeoutMinutes));

            if (offerResult.IsError)
            {
                _logger.LogWarning("Failed To Create Offer For Driver '{DriverId}': {Error}",
                    driverId, offerResult.TopError.Description);
                continue;
            }

            _context.TbServiceRequestOffers.Add(offerResult.Value);
            createdOffers.Add(offerResult.Value);
        }

        await _context.SaveChangesAsync(cancellation);

        foreach (var offer in createdOffers)
        {
            await _notifier.NotifyOfferAsync(offer.DriverId, offer.Id, serviceRequestId, cancellation);
        }

        _logger.LogInformation("Dispatched ServiceRequest '{Id}' To {Count} Drivers (Batch).",
            serviceRequestId, createdOffers.Count);

        return Result.Updated;
    }
}
