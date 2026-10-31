using NetTopologySuite.Geometries;
using UberSy.Domain.DeliveryRule.Enums;
using UberSy.Domain.ServiceRequests.Events.CreationEvents;
using UberSy.Domain.ServiceRequests.RideRequests;
using Xunit;

namespace UberSy.Domain.UnitTests.ServiceRequests.RideRequests;

public class RideRequestTests
{
    // ============================================================
    // Helper Methods
    // ============================================================
    private static Point CreateValidPoint()
    {
        return new Point(36.2765, 33.5138) { SRID = 4326 };
    }

    private static TbRideRequest CreateValidRideRequest()
    {
        return TbRideRequest.Create(
            id: Guid.NewGuid(),
            vehicleType: VehicleType.Motorcycle,
            serviceTier: ServiceTier.Standard,
            carType: null,
            receiverName: "John Doe",
            receiverPhone: "+963912345678",
            pickup: CreateValidPoint(),
            dropoff: CreateValidPoint(),
            distanceKm: 5.5m
        ).Value;
    }

    // ============================================================
    // Create Tests - Configuration
    // ============================================================

    [Fact]
    public void Create_ShouldReturnError_WhenVehicleTypeIsInvalid()
    {
        var result = TbRideRequest.Create(
            Guid.NewGuid(), (VehicleType)222, ServiceTier.Standard, null,
            null, null, CreateValidPoint(), CreateValidPoint(), 5m);

        Assert.False(result.IsSuccess);
        Assert.Equal(RideRequestErrors.VehicleTypeInvalid.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenPremiumTierIsNotCar()
    {
        var result = TbRideRequest.Create(
            Guid.NewGuid(), VehicleType.Motorcycle, ServiceTier.Premium, null,
            null, null, CreateValidPoint(), CreateValidPoint(), 5m);

        Assert.False(result.IsSuccess);
        Assert.Equal(RideRequestErrors.ServiceTierRequiresCar.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenCarTypeIsMissingForCar()
    {
        var result = TbRideRequest.Create(
            Guid.NewGuid(), VehicleType.Car, ServiceTier.Standard, null,
            null, null, CreateValidPoint(), CreateValidPoint(), 5m);

        Assert.False(result.IsSuccess);
        Assert.Equal(RideRequestErrors.CarTypeRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenCarTypeIsProvidedForNonCar()
    {
        var result = TbRideRequest.Create(
            Guid.NewGuid(), VehicleType.Motorcycle, ServiceTier.Standard, CarType.Taxi,
            null, null, CreateValidPoint(), CreateValidPoint(), 5m);

        Assert.False(result.IsSuccess);
        Assert.Equal(RideRequestErrors.CarTypeNotAllowed.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenPremiumCarIsNotPrivate()
    {
        var result = TbRideRequest.Create(
            Guid.NewGuid(), VehicleType.Car, ServiceTier.Premium, CarType.Taxi,
            null, null, CreateValidPoint(), CreateValidPoint(), 5m);

        Assert.False(result.IsSuccess);
        Assert.Equal(RideRequestErrors.CarTypePremiumRequiresPrivate.Code, result.TopError.Code);
    }

    // ============================================================
    // Create Tests - General Validation
    // ============================================================

    [Fact]
    public void Create_ShouldReturnError_WhenReceiverPhoneIsTooLong()
    {
        var result = TbRideRequest.Create(
            Guid.NewGuid(), VehicleType.Motorcycle, ServiceTier.Standard, null,
            "John", new string('1', 21), CreateValidPoint(), CreateValidPoint(), 5m);

        Assert.False(result.IsSuccess);
        Assert.Equal(RideRequestErrors.ReceiverPhoneTooLong.Code, result.TopError.Code);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Create_ShouldReturnError_WhenDistanceIsZeroOrNegative(decimal invalidDistance)
    {
        var result = TbRideRequest.Create(
            Guid.NewGuid(), VehicleType.Motorcycle, ServiceTier.Standard, null,
            null, null, CreateValidPoint(), CreateValidPoint(), invalidDistance);

        Assert.False(result.IsSuccess);
        Assert.Equal(RideRequestErrors.DistanceZero.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenPickupIsNull()
    {
        var result = TbRideRequest.Create(
            Guid.NewGuid(), VehicleType.Motorcycle, ServiceTier.Standard, null,
            null, null, null!, CreateValidPoint(), 5m);

        Assert.False(result.IsSuccess);
        Assert.Equal(RideRequestErrors.DistanceZero.Code, result.TopError.Code); // Based on your current code logic
    }

    [Fact]
    public void Create_ShouldReturnSuccess_AndRaiseDomainEvent()
    {
        // Arrange
        var id = Guid.NewGuid();
        var pickup = CreateValidPoint();
        var dropoff = CreateValidPoint();

        // Act
        var result = TbRideRequest.Create(
            id, VehicleType.Motorcycle, ServiceTier.Standard, null,
            "  John  ", "  +963912345678  ", pickup, dropoff, 10.5m);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("John", result.Value.ReceiverName); // Verify Trim
        Assert.Equal("+963912345678", result.Value.ReceiverPhone); // Verify Trim
        Assert.Equal(10.5m, result.Value.DistanceKm);
        Assert.Null(result.Value.DriverId);

        // Verify Domain Event
        Assert.Single(result.Value.DomainEvents);
        Assert.IsType<RideRequestCreated>(result.Value.DomainEvents.First());
    }

    // ============================================================
    // AssignDriver Tests
    // ============================================================

    [Fact]
    public void AssignDriver_ShouldReturnError_WhenDriverAlreadyAssigned()
    {
        var ride = CreateValidRideRequest();
        ride.AssignDriver(Guid.NewGuid()); // First time

        var result = ride.AssignDriver(Guid.NewGuid()); // Second time

        Assert.False(result.IsSuccess);
        Assert.Equal(RideRequestErrors.DriverAlreadyAssigned.Code, result.TopError.Code);
    }

    [Fact]
    public void AssignDriver_ShouldReturnSuccess_AndSetDriverId()
    {
        var ride = CreateValidRideRequest();
        var driverId = Guid.NewGuid();

        var result = ride.AssignDriver(driverId);

        Assert.True(result.IsSuccess);
        Assert.Equal(driverId, ride.DriverId);
    }

    // ============================================================
    // RecordFinancials Tests
    // ============================================================

    [Fact]
    public void RecordFinancials_ShouldReturnError_WhenDeliveryFeeIsNegative()
    {
        var ride = CreateValidRideRequest();
        var result = ride.RecordFinancials(-10m, 5m);

        Assert.False(result.IsSuccess);
        Assert.Equal(RideRequestErrors.DeliveryFeeNegative.Code, result.TopError.Code);
    }

    [Fact]
    public void RecordFinancials_ShouldReturnError_WhenDriverEarningExceedsDeliveryFee()
    {
        var ride = CreateValidRideRequest();
        var result = ride.RecordFinancials(10m, 15m);

        Assert.False(result.IsSuccess);
        Assert.Equal(RideRequestErrors.DriverEarningExceedsDeliveryFee.Code, result.TopError.Code);
    }

    [Fact]
    public void RecordFinancials_ShouldReturnSuccess_AndUpdateProperties()
    {
        var ride = CreateValidRideRequest();
        var deliveryFee = 20m;
        var driverEarning = 15m;

        var result = ride.RecordFinancials(deliveryFee, driverEarning);

        Assert.True(result.IsSuccess);
        Assert.Equal(deliveryFee, ride.DeliveryFee);
        Assert.Equal(driverEarning, ride.DriverEarning);
    }
}
