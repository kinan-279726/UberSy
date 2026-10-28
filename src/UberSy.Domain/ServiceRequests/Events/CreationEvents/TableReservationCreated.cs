using UberSy.Domain.Common;

namespace UberSy.Domain.ServiceRequests.Events.CreationEvents;

public sealed class TableReservationCreated : DomainEvent
{
    public Guid ServiceRequestId { get; }
    public Guid CustomerId { get; }
    public Guid RestaurantId { get; }
    public DateTime ReservationTimeUtc { get; }
    public int PartySize { get; }
    public decimal DepositAmount { get; }
    public DateTime ResponseDeadlineUtc { get; }

    public TableReservationCreated(
        Guid serviceRequestId,
        Guid customerId,
        Guid restaurantId,
        DateTime reservationTimeUtc,
        int partySize,
        decimal depositAmount,
        DateTime responseDeadlineUtc)
    {
        ServiceRequestId = serviceRequestId;
        CustomerId = customerId;
        RestaurantId = restaurantId;
        ReservationTimeUtc = reservationTimeUtc;
        PartySize = partySize;
        DepositAmount = depositAmount;
        ResponseDeadlineUtc = responseDeadlineUtc;
    }
}