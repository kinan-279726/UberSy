using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.ServiceRequests.Offers.Commands.AcceptOffer;

public sealed class AcceptOfferCommandHandler(IApplicationDbContext context, IUserNotifier notifier, ILogger<AcceptOfferCommandHandler> logger, IOwnershipValidator ownership)
    : IRequestHandler<AcceptOfferCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IUserNotifier _notifier = notifier;
    private readonly ILogger<AcceptOfferCommandHandler> _logger = logger;
    private readonly IOwnershipValidator _ownership = ownership;

    public async Task<Result<Updated>> Handle(AcceptOfferCommand request, CancellationToken cancellationToken)
    {
        var driverResult = await _ownership.IsDriver();
        if (driverResult.IsError) return driverResult.Errors;

        var offer = await _context.TbServiceRequestOffers.FirstOrDefaultAsync(
            o => o.Id == request.OfferId,
            cancellationToken);

        if (offer is null) return ApplicationErrors.OfferNotFound;

        var driverId = driverResult.Value;
        if (driverId != offer.DriverId)
        {
            return ApplicationErrors.AccessDenied;
        }

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

        var result = await _context.TbServiceRequests
            .AsNoTracking()
            .Where(s => s.Id == request.OfferId)
            .Select(s => new { s.CustomerId, s.Status })
            .FirstOrDefaultAsync(cancellationToken);

        if (result is not null)
            await _notifier.NotifyDriverAssigned(offer.ServiceRequestId, result.CustomerId);

        _logger.LogInformation("Offer '{OfferId}' Accepted By Driver '{DriverId}'.", request.OfferId, driverId);

        return Result.Updated;
    }
}
