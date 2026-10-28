using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.ServiceRequests.Events.LifecycleEvents;

namespace UberSy.Application.Features.ServiceRequests.EventHandlers;

public class OutForDeliveryHandler(IUserNotifier notifier , ILogger<OutForDeliveryHandler> logger) 
    : INotificationHandler<ServiceRequestOutForDelivery>
{
    private readonly IUserNotifier _notifier = notifier;
    private readonly ILogger<OutForDeliveryHandler> _logger = logger;

    public async Task Handle(ServiceRequestOutForDelivery notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("user with id {id} notify out fro delivery service id {servId}", notification.CustomerId, notification.ServiceRequestId);

       await _notifier.NotifyServiceOutForDelivery(notification.ServiceRequestId, notification.CustomerId);
    }
}
