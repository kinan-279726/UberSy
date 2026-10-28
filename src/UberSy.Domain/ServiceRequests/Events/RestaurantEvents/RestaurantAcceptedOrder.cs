using UberSy.Domain.Common;

namespace UberSy.Domain.ServiceRequests.Events.RestaurantEvents;

public sealed class RestaurantAcceptedOrder : DomainEvent
{
    public Guid ServiceRequestId { get; }
    public Guid RestaurantId { get; }
    public Guid CustomerId { get; }
    public int EstimatedPrepMinutes { get; }

    public RestaurantAcceptedOrder(
        Guid serviceRequestId,
        Guid restaurantId,
        Guid customerId ,
        int estimatedPrepMinutes)
    {
        ServiceRequestId = serviceRequestId;
        RestaurantId = restaurantId;
        CustomerId = customerId;
        EstimatedPrepMinutes = estimatedPrepMinutes;
    }
}
