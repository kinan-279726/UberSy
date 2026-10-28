using UberSy.Domain.Common;

namespace UberSy.Domain.ServiceRequests.Events.DriverEvents;

public sealed class DriverRejectedOffer : DomainEvent
{
    public Guid ServiceRequestId { get; }
    public Guid OfferId { get; }
    public Guid DriverId { get; }

    public DriverRejectedOffer(
        Guid serviceRequestId,
        Guid offerId,
        Guid driverId)
    {
        ServiceRequestId = serviceRequestId;
        OfferId = offerId;
        DriverId = driverId;
    }
}
