using UberSy.Domain.Common;

namespace UberSy.Domain.ServiceRequests.Events.LifecycleEvents;

public sealed class ServiceRequestPreparing : DomainEvent
{
    public Guid ServiceRequestId { get; }
    public Guid CustomerId { get; }

    public ServiceRequestPreparing(
        Guid serviceRequestId,
        Guid customerId)
    {
        ServiceRequestId = serviceRequestId;
        CustomerId = customerId;
    }
}
