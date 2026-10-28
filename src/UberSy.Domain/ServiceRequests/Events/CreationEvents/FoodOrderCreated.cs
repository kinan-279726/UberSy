using UberSy.Domain.Common;

namespace UberSy.Domain.ServiceRequests.Events.CreationEvents;

public sealed class FoodOrderCreated : DomainEvent
{
    public Guid ServiceRequestId { get; }

    public Guid RestaurantId { get; }

    public Guid AddressId { get; }

    public DateTimeOffset ResponseDeadlineUtc { get; }

    public FoodOrderCreated(
        Guid serviceRequestId,
        Guid restaurantId,
        Guid addressId,
        DateTimeOffset responseDeadlineUtc)
    {
        ServiceRequestId = serviceRequestId;
        RestaurantId = restaurantId;
        AddressId = addressId!;
        ResponseDeadlineUtc = responseDeadlineUtc;
    }
}
