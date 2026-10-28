using UberSy.Domain.Common;

namespace UberSy.Domain.ServiceRequests.Events.CreationEvents;

public sealed class RideRequestCreated : DomainEvent
{
    public Guid ServiceRequestId { get; }

    public RideRequestCreated(Guid serviceRequestId)
    {
        ServiceRequestId = serviceRequestId;
    }
}