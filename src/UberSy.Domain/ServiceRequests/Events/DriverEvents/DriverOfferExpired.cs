using UberSy.Domain.Common;

namespace UberSy.Domain.ServiceRequests.Events.DriverEvents;

public sealed class DriverOfferExpired : DomainEvent
{
    public Guid ServiceRequestId { get; }
    public Guid OfferId { get; }
    public Guid DriverId { get; }

    public DriverOfferExpired(
        Guid serviceRequestId,
        Guid offerId,
        Guid driverId)
    {
        ServiceRequestId = serviceRequestId;
        OfferId = offerId;
        DriverId = driverId;
    }
}