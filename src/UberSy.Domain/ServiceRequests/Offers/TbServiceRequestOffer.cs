using UberSy.Domain.Common;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Drivers;
using UberSy.Domain.ServiceRequests.Events;
using UberSy.Domain.ServiceRequests.Events.DriverEvents;
using UberSy.Domain.ServiceRequests.Offers.Enums;

namespace UberSy.Domain.ServiceRequests.Offers;

public sealed class TbServiceRequestOffer : AuditableEntity
{
    public Guid ServiceRequestId { get; private set; }
    public Guid DriverId { get; private set; }
    public OfferStatus Status { get; private set; }
    public DateTimeOffset SentAt { get; private set; }
    public DateTimeOffset ResponseTimeout { get; private set; }
    public DateTimeOffset? RespondedAt { get; private set; }
    public TbDriver Driver { get; private set; } = null!;
    public TbServiceRequest ServiceRequest { get; private set; } = null!;

    private TbServiceRequestOffer()
    { }

    private TbServiceRequestOffer(Guid id, Guid serviceRequestId, Guid driverId , DateTimeOffset responseTimeout)
        : base(id)
    {
        ServiceRequestId = serviceRequestId;
        DriverId = driverId;
        Status = OfferStatus.Sent;
        SentAt = DateTimeOffset.UtcNow;
        ResponseTimeout = responseTimeout;
    }

    public static Result<TbServiceRequestOffer> Create(Guid id, Guid serviceRequestId, Guid driverId , DateTimeOffset responseTimeout)
    {
        if (driverId == Guid.Empty) return OfferErrors.DriverIdRequired;

        var offer = new TbServiceRequestOffer(id, serviceRequestId, driverId , responseTimeout);

        offer.AddDomainEvent(new ServiceRequestOfferSent(id, serviceRequestId, driverId));

        return offer;

    }

    public Result<Updated> Accept()
    {
        if (Status != OfferStatus.Sent) return OfferErrors.OfferAlreadyRespondedTo;

        Status = OfferStatus.Accepted;
        RespondedAt = DateTimeOffset.UtcNow;

        AddDomainEvent(new DriverAcceptedOffer(ServiceRequestId , Id , DriverId));

        return Result.Updated;
    }

    public Result<Updated> Reject()
    {
        if (Status != OfferStatus.Sent) return OfferErrors.OfferAlreadyRespondedTo;

        Status = OfferStatus.Rejected;
        RespondedAt = DateTimeOffset.UtcNow;

        AddDomainEvent(new DriverRejectedOffer(Id, ServiceRequestId, DriverId));

        return Result.Updated;
    }

    public Result<Updated> Expire()
    {
        if (Status != OfferStatus.Sent) return OfferErrors.OfferAlreadyRespondedTo;

        Status = OfferStatus.Expired;

        AddDomainEvent(new DriverOfferExpired(Id, ServiceRequestId, DriverId));

        return Result.Updated;
    }

    public Result<Updated> MarkAsLostToAnotherDriver()
    {
        if (Status != OfferStatus.Sent)
        {
            return Result.Updated;
        }

        Status = OfferStatus.LostToAnotherDriver;

        return Result.Updated;
    }
}