namespace UberSy.Application.Common.Interfaces;

public interface IRestaurantNotifier
{
    Task NotifyNewOrderAsync(Guid restaurantId, Guid serviceRequestId, CancellationToken ct = default);

    Task NotifyOrderCancledAsync(Guid restaurantId, Guid serviceRequestId, CancellationToken ct = default);
}
