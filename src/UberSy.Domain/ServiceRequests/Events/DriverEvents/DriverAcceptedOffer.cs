using UberSy.Domain.Common;

namespace UberSy.Domain.ServiceRequests.Events.DriverEvents;

public sealed class DriverAcceptedOffer : DomainEvent
{
    public Guid ServiceRequestId { get; }
    public Guid OfferId { get; }
    public Guid DriverId { get; }

    public DriverAcceptedOffer(
        Guid serviceRequestId,
        Guid offerId,
        Guid driverId)
    {
        ServiceRequestId = serviceRequestId;
        OfferId = offerId;
        DriverId = driverId;
    }
}
