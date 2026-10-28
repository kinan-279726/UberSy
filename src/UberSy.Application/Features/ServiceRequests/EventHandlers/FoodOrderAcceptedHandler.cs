using MediatR;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.ServiceRequests.Events.LifecycleEvents;

namespace UberSy.Application.Features.ServiceRequests.EventHandlers
{
    public sealed class FoodOrderAcceptedHandler(IUserNotifier notifier) : INotificationHandler<ServiceRequestPreparing>
    {
        private readonly IUserNotifier _notifier = notifier;

        public async Task Handle(ServiceRequestPreparing notification, CancellationToken cancellationToken)
        {
           await _notifier.NotifyServiceAcceptedByRestaurant(notification.ServiceRequestId, notification.CustomerId);
        }
    }
}
