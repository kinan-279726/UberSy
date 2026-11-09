using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.ServiceRequests.Events.LifecycleEvents;
using UberSy.Domain.ServiceRequests.Offers.Enums;

namespace UberSy.Application.Features.ServiceRequests.EventHandlers;

public sealed class ServiceRequestCancelledHandler(
    IApplicationDbContext context,
    IDriverNotifier notifier,
    ILogger<ServiceRequestCancelledHandler> logger)
    : INotificationHandler<ServiceRequestCancelled>
{
    public async Task Handle(ServiceRequestCancelled notification, CancellationToken cancellationToken)
    {
        var offers = await context.TbServiceRequestOffers
            .Where(o => o.ServiceRequestId == notification.ServiceRequestId && o.Status == OfferStatus.Sent)
            .ToListAsync(cancellationToken);

        foreach (var offer in offers) offer.Expire();

        var assignedDriverId = await context.TbFoodOrders
            .AsNoTracking()
            .Where(o => o.Id == notification.ServiceRequestId)
            .Select(o => o.DriverId)
            .FirstOrDefaultAsync(cancellationToken);

        Guid? releasedDriverId = null;

        if (assignedDriverId is not null)
        {
            var driver = await context.TbDrivers
                .FirstOrDefaultAsync(d => d.Id == assignedDriverId, cancellationToken);

            if (driver is not null)
            {
                var finishResult = driver.FinishDelivery();

                if (finishResult.IsError)
                {
                    logger.LogWarning("Could Not Reset Driver '{DriverId}' Status After Cancelling Order '{OrderId}': {Error}",
                        driver.Id, notification.ServiceRequestId, finishResult.TopError.Description);
                }
                else
                {
                    releasedDriverId = driver.Id;
                }
            }
        }

        if (offers.Count == 0 && releasedDriverId is null) return;

        await context.SaveChangesAsync(cancellationToken);

        // 3) Notify every driver affected
        foreach (var offer in offers)
            await notifier.NotifyOrderCanceldAsync(offer.DriverId, cancellationToken);

        if (releasedDriverId is not null)
            await notifier.NotifyOrderCanceldAsync(releasedDriverId.Value, cancellationToken);
    }
}
