using UberSy.Domain.Common;

namespace UberSy.Domain.ServiceRequests.Events.RestaurantEvents;

public sealed class RestaurantRejectedOrder : DomainEvent
{
    public Guid ServiceRequestId { get; }
    public Guid RestaurantId { get; }
    public string Reason { get; }

    public RestaurantRejectedOrder(
        Guid serviceRequestId,
        Guid restaurantId,
        string reason)
    {
        ServiceRequestId = serviceRequestId;
        RestaurantId = restaurantId;
        Reason = reason;
    }
}
