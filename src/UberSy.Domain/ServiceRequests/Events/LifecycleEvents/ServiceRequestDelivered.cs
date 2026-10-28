using UberSy.Domain.Common;

namespace UberSy.Domain.ServiceRequests.Events.LifecycleEvents;

public sealed class ServiceRequestDelivered : DomainEvent
{
    public Guid ServiceRequestId { get; }

    public Guid CustomerId { get; }

    public ServiceRequestDelivered(
        Guid serviceRequestId,
        Guid customerId)
    {
        ServiceRequestId = serviceRequestId;
        CustomerId = customerId;
    }
}
