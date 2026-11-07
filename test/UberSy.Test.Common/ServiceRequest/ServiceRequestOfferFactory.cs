using UberSy.Domain.Common.Results;
using UberSy.Domain.ServiceRequests.Offers;

namespace UberSy.Test.Common.ServiceRequest;

public static class ServiceRequestOfferFactory
{
    public static Result<TbServiceRequestOffer> CreateServiceRequestOffer(
        Guid? id = null,
        Guid? serviceRequestId = null,
        Guid? driverId = null,
        DateTimeOffset? responseTimeout = null)
    {
        return TbServiceRequestOffer.Create(
            id ?? Guid.NewGuid(),
            serviceRequestId ?? Guid.NewGuid(),
            driverId ?? Guid.NewGuid(),
            responseTimeout ?? DateTimeOffset.UtcNow.AddMinutes(2));
    }
}
