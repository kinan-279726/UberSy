namespace UberSy.Application.Common.Interfaces;

public interface IUserNotifier
{
    Task NotifyServiceOutForDelivery(Guid serviceRequestId, Guid customerId, CancellationToken cancellationToken = default);

    Task NotifyServiceCancelledRestaurant(Guid serviceRequestId, Guid customerId, CancellationToken cancellationToken = default);

    Task NotifyServiceAcceptedByRestaurant(Guid ServiceRequestId, Guid customerId, CancellationToken cancellationToken = default);

    Task NotifyServiceCancelledBySystem(Guid serviceRequestId, Guid customerId, CancellationToken cancellationToken = default);

    Task NotifyServiceDelivered(Guid serviceRequestId, Guid customerId, CancellationToken cancellationToken = default);

    Task NotifyDriverAssigned(Guid serviceRequestId, Guid customerId, CancellationToken cancellationToken = default);

    Task NotifyDriverLocationAsync(Guid serviceRequestId, Guid customerId, double latitude, double longitude, CancellationToken cancellationToken = default);
}
