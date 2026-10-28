using UberSy.Domain.Common;

namespace UberSy.Domain.ServiceRequests.Events.LifecycleEvents;

public sealed class ServiceRequestCancelled : DomainEvent
{
    public Guid ServiceRequestId { get; }

    public ServiceRequestCancelled(
        Guid serviceRequestId)
    {
        ServiceRequestId = serviceRequestId;
    }
}