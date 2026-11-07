using NetTopologySuite.Geometries;
using UberSy.Domain.Common.Results;
using UberSy.Domain.DeliveryRule.Enums;
using UberSy.Domain.ServiceRequests.RideRequests;

namespace UberSy.Test.Common.ServiceRequest;

public static class RideRequestFactory
{
    public static Result<TbRideRequest> CreateRideRequest(
        Guid? id = null,
        VehicleType? vehicleType = null,
        ServiceTier? serviceTier = null,
        CarType? carType = null,
        string? receiverName = null,
        string? receiverPhone = null,
        Point? pickup = null,
        Point? dropoff = null,
        decimal? distanceKm = null)
    {
        var vt = vehicleType ?? VehicleType.Motorcycle;

        return TbRideRequest.Create(
            id ?? Guid.NewGuid(),
            vt,
            serviceTier ?? ServiceTier.Standard,
            carType,
            receiverName,
            receiverPhone,
            pickup ?? new Point(36.2765, 33.5138) { SRID = 4326 },
            dropoff ?? new Point(36.2900, 33.5200) { SRID = 4326 },
            distanceKm ?? 5.5m);
    }
}
