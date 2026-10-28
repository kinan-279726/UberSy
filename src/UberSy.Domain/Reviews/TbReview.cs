using UberSy.Domain.Common;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Drivers;
using UberSy.Domain.Restaurants;
using UberSy.Domain.ServiceRequests;

namespace UberSy.Domain.Reviews;

public sealed class TbReview : AuditableEntity
{
    public Guid CustomerId { get; private set; }
    public Guid ServiceRequestId { get; private set; }
    public Guid? RestaurantId { get; private set; }
    public Guid? DriverId { get; private set; }
    public byte Rating { get; private set; }
    public string? Note { get; private set; }

    public TbDriver? Driver { get; private set; }
    public TbRestaurant? Restaurant { get; private set; }
    public TbServiceRequest ServiceRequest { get; private set; } = null!;

    private TbReview()
    { }

    private TbReview(Guid id, Guid customerId, Guid serviceRequestId, Guid? restaurantId, Guid? driverId, byte rating, string? note)
        : base(id)
    {
        CustomerId = customerId;
        ServiceRequestId = serviceRequestId;
        RestaurantId = restaurantId;
        DriverId = driverId;
        Rating = rating;
        Note = note;
    }

    public static Result<TbReview> Create(Guid id, Guid customerId, Guid serviceRequestId, Guid? restaurantId, Guid? driverId, byte rating, string? note)
    {
        if (customerId == Guid.Empty) return ReviewErrors.CustomerIdRequired;
        if (serviceRequestId == Guid.Empty) return ReviewErrors.ServiceRequestIdRequired;

        var hasRestaurant = restaurantId is not null;
        var hasDriver = driverId is not null;
        if (hasRestaurant == hasDriver) return ReviewErrors.ExactlyOneTargetRequired;

        if (rating is < 1 or > 5) return ReviewErrors.RatingOutOfRange;
        if (note is { Length: > 1000 }) return ReviewErrors.NoteTooLong;

        return new TbReview(id, customerId, serviceRequestId, restaurantId, driverId, rating, note?.Trim());
    }
}