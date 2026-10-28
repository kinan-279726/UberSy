using UberSy.Domain.Common;

namespace UberSy.Domain.ServiceRequests.Events.RestaurantEvents;

public sealed class RestaurantResponseTimedOut : DomainEvent
{
    public Guid ServiceRequestId { get; }
    public Guid RestaurantId { get; }
    public int ResponseTimeoutMinutes { get; }

    public RestaurantResponseTimedOut(
        Guid serviceRequestId,
        Guid restaurantId,
        int timeoutMinutes)
    {
        ServiceRequestId = serviceRequestId;
        RestaurantId = restaurantId;
        ResponseTimeoutMinutes = timeoutMinutes;
    }
}
