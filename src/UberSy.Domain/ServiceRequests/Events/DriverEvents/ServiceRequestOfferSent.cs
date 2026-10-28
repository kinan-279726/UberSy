using UberSy.Domain.Common;

namespace UberSy.Domain.ServiceRequests.Events;

public sealed class ServiceRequestOfferSent : DomainEvent
{
    public Guid ServiceRequestId { get; }
    public Guid OfferId { get; }
    public Guid DriverId { get; }

    public ServiceRequestOfferSent(
        Guid serviceRequestId,
        Guid offerId,
        Guid driverId)
    {
        ServiceRequestId = serviceRequestId;
        OfferId = offerId;
        DriverId = driverId;
    }
}