using UberSy.Domain.Common.Results;
using UberSy.Domain.ServiceRequests;
using UberSy.Domain.ServiceRequests.Enums;

namespace UberSy.Test.Common.ServiceRequest;

public static class ServiceRequestFactory
{
    public static Result<TbServiceRequest> CreateServiceRequest(
        Guid? id = null,
        Guid? customerId = null,
        ServiceType? serviceType = null,
        decimal? totalAmount = null)
    {
        return TbServiceRequest.Create(
            id ?? Guid.NewGuid(),
            customerId ?? Guid.NewGuid(),
            serviceType ?? ServiceType.Food,
            totalAmount ?? 100m);
    }
}
