using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;
using UberSy.Domain.ServiceRequests.Enums;

namespace UberSy.Application.Features.ServiceRequests.Offers.Commands.AcceptOffer;

public sealed class AcceptOfferCommandHandler(
    IApplicationDbContext context,
    IUserNotifier notifier,
    ILogger<AcceptOfferCommandHandler> logger,
    IOwnershipValidator ownership,
    IDeliveryPricingService pricing)
    : IRequestHandler<AcceptOfferCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IUserNotifier _notifier = notifier;
    private readonly ILogger<AcceptOfferCommandHandler> _logger = logger;
    private readonly IOwnershipValidator _ownership = ownership;
    private readonly IDeliveryPricingService _pricing = pricing;

    public async Task<Result<Updated>> Handle(AcceptOfferCommand request, CancellationToken cancellationToken)
    {
        var driverResult = await _ownership.IsDriver();
        if (driverResult.IsError) return driverResult.Errors;
        var driverId = driverResult.Value;

        var offer = await _context.TbServiceRequestOffers
            .FirstOrDefaultAsync(o => o.Id == request.OfferId, cancellationToken);

        if (offer is null) return ApplicationErrors.OfferNotFound;
        if (driverId != offer.DriverId) return ApplicationErrors.AccessDenied;

        if (offer.ResponseTimeout <= DateTimeOffset.UtcNow)
            return ApplicationErrors.OfferNoLongerAvailable;

        var requestStatus = await _context.TbServiceRequests
            .AsNoTracking()
            .Where(s => s.Id == offer.ServiceRequestId)
            .Select(s => (ServiceRequestStatus?)s.Status)
            .FirstOrDefaultAsync(cancellationToken);

        if (requestStatus != ServiceRequestStatus.Preparing)
            return ApplicationErrors.OfferNoLongerAvailable;

        var foodOrder = await _context.TbFoodOrders
            .AsNoTracking()
            .Include(o => o.Restaurant)
            .Include(o => o.Address)
            .FirstOrDefaultAsync(o => o.Id == offer.ServiceRequestId, cancellationToken);

        if (foodOrder is null) return ApplicationErrors.ServiceRequestNotFound;

        var driver = await _context.TbDrivers
            .AsNoTracking()
            .FirstAsync(d => d.Id == driverId, cancellationToken);

        var quote = await _pricing.CalculateAsync(
            foodOrder.Restaurant.Boundary, foodOrder.Address.Boundary,
            driver.VehicleType, driver.CarType, cancellationToken);

        if (quote.IsError) return quote.Errors;

        var offerResult = offer.Accept();
        if (offerResult.IsError) return offerResult.Errors;

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            _logger.LogInformation(
                "Offer '{OfferId}' Was Already Handled By Another Process When Driver '{DriverId}' Tried To Accept.",
                request.OfferId, driverId);

            return ApplicationErrors.OfferNoLongerAvailable;
        }

        var assignedDriverId = await _context.TbFoodOrders
            .AsNoTracking()
            .Where(o => o.Id == offer.ServiceRequestId)
            .Select(o => o.DriverId)
            .FirstOrDefaultAsync(cancellationToken);

        if (assignedDriverId != driverId)
        {
            _logger.LogWarning(
                "Offer '{OfferId}' Was Accepted By Driver '{DriverId}' But The Order Was Not Assigned To Him.",
                request.OfferId, driverId);

            return ApplicationErrors.OfferNoLongerAvailable;
        }

        var customerId = await _context.TbServiceRequests
            .AsNoTracking()
            .Where(s => s.Id == offer.ServiceRequestId)
            .Select(s => s.CustomerId)
            .FirstAsync(cancellationToken);

        await _notifier.NotifyDriverAssigned(offer.ServiceRequestId, customerId, cancellationToken);

        _logger.LogInformation("Offer '{OfferId}' Accepted By Driver '{DriverId}'.", request.OfferId, driverId);

        return Result.Updated;
    }
}
