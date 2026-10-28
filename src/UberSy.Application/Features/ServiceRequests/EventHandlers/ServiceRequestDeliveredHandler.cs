using MediatR;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.ServiceRequests.Events.LifecycleEvents;

namespace UberSy.Application.Features.ServiceRequests.EventHandlers;

public sealed class ServiceRequestDeliveredHandler(
    IUserNotifier notifier)
    : INotificationHandler<ServiceRequestDelivered>
{
    private readonly IUserNotifier _notifier = notifier;

    public async Task Handle(ServiceRequestDelivered notification, CancellationToken cancellation)
    {
        await _notifier.NotifyServiceDelivered(notification.ServiceRequestId, notification.CustomerId);
    }
}
