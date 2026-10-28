using UberSy.Domain.Common;

namespace UberSy.Domain.ServiceRequests.Events.LifecycleEvents;

public sealed class ServiceRequestOutForDelivery :DomainEvent
{
    public Guid ServiceRequestId { get; }

    public Guid CustomerId { get; }

    public ServiceRequestOutForDelivery(
        Guid serviceRequestId, Guid customerId)
    {
        ServiceRequestId = serviceRequestId;
        CustomerId = customerId;
    }
}
