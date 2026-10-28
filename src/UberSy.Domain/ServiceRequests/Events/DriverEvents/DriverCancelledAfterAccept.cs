using UberSy.Domain.Common;

namespace UberSy.Domain.ServiceRequests.Events.DriverEvents;

public sealed class DriverCancelledAfterAccept : DomainEvent
{
    public Guid ServiceRequestId { get; }
    public Guid DriverId { get; }

    public DriverCancelledAfterAccept(
        Guid serviceRequestId,
        Guid driverId)
    {
        ServiceRequestId = serviceRequestId;
        DriverId = driverId;
    }
}
