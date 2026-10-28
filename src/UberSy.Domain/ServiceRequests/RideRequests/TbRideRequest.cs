using NetTopologySuite.Geometries;
using UberSy.Domain.Common;
using UberSy.Domain.Common.Results;
using UberSy.Domain.DeliveryRule.Enums;
using UberSy.Domain.Drivers;
using UberSy.Domain.ServiceRequests.Events.CreationEvents;

namespace UberSy.Domain.ServiceRequests.RideRequests;

public sealed class TbRideRequest : AuditableEntity
{
    public Guid? DriverId { get; private set; }
    public VehicleType RequestedVehicleType { get; private set; }
    public ServiceTier RequestedServiceTier { get; private set; }
    public CarType? RequestedCarType { get; private set; }
    public string? ReceiverName { get; private set; }
    public string? ReceiverPhone { get; private set; }
    public Point PickupBoundary { get; private set; } = null!;
    public Point DropoffBoundary { get; private set; } = null!;
    public decimal DistanceKm { get; private set; }
    public decimal DeliveryFee { get; private set; }
    public decimal DriverEarning { get; private set; }

    public TbDriver? Driver { get; private set; }
    public TbServiceRequest IdNavigation { get; private set; } = null!;

    private TbRideRequest()
    { }

    private TbRideRequest(Guid id, VehicleType vehicleType, ServiceTier serviceTier, CarType? carType,
        string? receiverName, string? receiverPhone, Point pickup, Point dropoff, decimal distanceKm)
        : base(id)
    {
        RequestedVehicleType = vehicleType;
        RequestedServiceTier = serviceTier;
        RequestedCarType = carType;
        ReceiverName = receiverName;
        ReceiverPhone = receiverPhone;
        PickupBoundary = pickup;
        DropoffBoundary = dropoff;
        DistanceKm = distanceKm;
    }

    public static Result<TbRideRequest> Create(Guid id, VehicleType vehicleType, ServiceTier serviceTier, CarType? carType,
        string? receiverName, string? receiverPhone, Point pickup, Point dropoff, decimal distanceKm)
    {
        var config = ValidateConfiguration(vehicleType, serviceTier, carType);
        if (config.IsError) return config.Errors;

        if (receiverPhone is { Length: > 20 }) return RideRequestErrors.ReceiverPhoneTooLong;
        if (distanceKm <= 0) return RideRequestErrors.DistanceZero;
        if (pickup is null || !pickup.IsValid) return RideRequestErrors.DistanceZero;
        if (dropoff is null || !dropoff.IsValid) return RideRequestErrors.DistanceZero;

        var ride = new TbRideRequest(id, vehicleType, serviceTier, carType, receiverName?.Trim(), receiverPhone?.Trim(),
            pickup, dropoff, distanceKm);

        ride.AddDomainEvent(new RideRequestCreated(
            serviceRequestId: id
        ));

        return ride;
    }

    public Result<Updated> AssignDriver(Guid driverId)
    {
        if (DriverId is not null) return RideRequestErrors.DriverAlreadyAssigned;

        DriverId = driverId;

        return Result.Updated;
    }

    public Result<Updated> RecordFinancials(decimal deliveryFee, decimal driverEarning)
    {
        if (deliveryFee < 0) return RideRequestErrors.DeliveryFeeNegative;
        if (driverEarning > deliveryFee) return RideRequestErrors.DriverEarningExceedsDeliveryFee;

        DeliveryFee = deliveryFee;
        DriverEarning = driverEarning;

        return Result.Updated;
    }

    private static Result<Success> ValidateConfiguration(VehicleType vehicleType, ServiceTier serviceTier, CarType? carType)
    {
        if (!Enum.IsDefined(vehicleType)) return RideRequestErrors.VehicleTypeInvalid;
        if (!Enum.IsDefined(serviceTier)) return RideRequestErrors.ServiceTierInvalid;

        if (serviceTier == ServiceTier.Premium && vehicleType != VehicleType.Car)
            return RideRequestErrors.ServiceTierRequiresCar;

        if (vehicleType == VehicleType.Car && carType is null) return RideRequestErrors.CarTypeRequired;
        if (vehicleType != VehicleType.Car && carType is not null) return RideRequestErrors.CarTypeNotAllowed;

        if (serviceTier == ServiceTier.Premium && carType != DeliveryRule.Enums.CarType.Private)
            return RideRequestErrors.CarTypePremiumRequiresPrivate;

        return Result.Success;
    }
}