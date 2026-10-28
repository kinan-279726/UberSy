using UberSy.Domain.Common;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Restaurants;
using UberSy.Domain.ServiceRequests;

namespace UberSy.Domain.Reservations;

public sealed class TbTableReservation : Entity
{
    public Guid RestaurantId { get; private set; }
    public DateTime ReservationTime { get; private set; }
    public int PartySize { get; private set; }
    public decimal DepositAmount { get; private set; }
    public string? Note { get; private set; }

    public TbServiceRequest IdNavigation { get; private set; } = null!;
    public TbRestaurant Restaurant { get; private set; } = null!;

    private TbTableReservation()
    { }

    private TbTableReservation(Guid id, Guid restaurantId, DateTime reservationTime, int partySize, decimal depositAmount, string? note)
        : base(id)
    {
        RestaurantId = restaurantId;
        ReservationTime = reservationTime;
        PartySize = partySize;
        DepositAmount = depositAmount;
        Note = note;
    }

    public static Result<TbTableReservation> Create(Guid id, Guid restaurantId, DateTime reservationTime, int partySize, decimal depositAmount, string? note)
    {
        if (restaurantId == Guid.Empty) return TableReservationErrors.RestaurantIdRequired;
        if (reservationTime <= DateTime.UtcNow) return TableReservationErrors.ReservationTimeMustBeFuture;
        if (partySize <= 0) return TableReservationErrors.PartySizeZero;
        if (depositAmount < 0) return TableReservationErrors.DepositAmountNegative;
        if (note is { Length: > 1000 }) return TableReservationErrors.NoteTooLong;

        return new TbTableReservation(id, restaurantId, reservationTime, partySize, depositAmount, note?.Trim());
    }

    public Result<Updated> Reschedule(DateTime reservationTime, int partySize)
    {
        if (reservationTime <= DateTime.UtcNow) return TableReservationErrors.ReservationTimeMustBeFuture;
        if (partySize <= 0) return TableReservationErrors.PartySizeZero;

        ReservationTime = reservationTime;
        PartySize = partySize;

        return Result.Updated;
    }
}