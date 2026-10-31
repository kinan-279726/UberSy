using UberSy.Domain.DeliveryRule.Enums;
using UberSy.Domain.Drivers;
using UberSy.Domain.Drivers.Enums;
using Xunit;

namespace UberSy.Domain.UnitTests.Drivers;

public class DriverTests
{
    // ============================================================
    // Helper Methods
    // ============================================================
    private static TbDriver CreateValidDriver()
    {
        // Default is a valid Motorcycle driver
        return TbDriver.Create(
            id: Guid.NewGuid(),
            userId: Guid.NewGuid(),
            vehicleType: VehicleType.Motorcycle,
            serviceTier: ServiceTier.Standard,
            carType: null,
            nationalId: "12345678901",
            photoUrl: null,
            licenseNumber: "LIC-123",
            plateNumber: "PL-123",
            model: "Honda",
            color: "Black"
        ).Value;
    }

    // ============================================================
    // Create Tests - General Validation
    // ============================================================

    [Fact]
    public void Create_ShouldReturnError_WhenUserIdIsEmpty()
    {
        var result = TbDriver.Create(
            Guid.NewGuid(), Guid.Empty, VehicleType.Motorcycle, ServiceTier.Standard, null,
            "12345678901", null, "LIC-123", "PL-123", "Honda", "Black");

        Assert.False(result.IsSuccess);
        Assert.Equal(DriverErrors.UserIdRequired.Code, result.TopError.Code);
    }

    // ============================================================
    // Create Tests - Vehicle Configuration
    // ============================================================

    [Fact]
    public void Create_ShouldReturnError_WhenPremiumTierIsNotCar()
    {
        var result = TbDriver.Create(
            Guid.NewGuid(), Guid.NewGuid(), VehicleType.Motorcycle, ServiceTier.Premium, null,
            "12345678901", null, "LIC-123", "PL-123", "Honda", "Black");

        Assert.False(result.IsSuccess);
        Assert.Equal(DriverErrors.ServiceTierRequiresCar.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenCarTypeIsMissingForCar()
    {
        var result = TbDriver.Create(
            Guid.NewGuid(), Guid.NewGuid(), VehicleType.Car, ServiceTier.Standard, null,
            "12345678901", null, "LIC-123", "PL-123", "Toyota", "White");

        Assert.False(result.IsSuccess);
        Assert.Equal(DriverErrors.CarTypeRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenCarTypeIsProvidedForNonCar()
    {
        var result = TbDriver.Create(
            Guid.NewGuid(), Guid.NewGuid(), VehicleType.Motorcycle, ServiceTier.Standard, CarType.Taxi,
            "12345678901", null, "LIC-123", "PL-123", "Honda", "Black");

        Assert.False(result.IsSuccess);
        Assert.Equal(DriverErrors.CarTypeNotAllowed.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenPremiumCarIsNotPrivate()
    {
        var result = TbDriver.Create(
            Guid.NewGuid(), Guid.NewGuid(), VehicleType.Car, ServiceTier.Premium, CarType.Taxi,
            "12345678901", null, "LIC-123", "PL-123", "Mercedes", "Black");

        Assert.False(result.IsSuccess);
        Assert.Equal(DriverErrors.CarTypePremiumRequiresPrivate.Code, result.TopError.Code);
    }

    // ============================================================
    // Create Tests - Documents Validation
    // ============================================================

    [Fact]
    public void Create_ShouldReturnError_WhenNationalIdIsMissing()
    {
        var result = TbDriver.Create(
            Guid.NewGuid(), Guid.NewGuid(), VehicleType.Motorcycle, ServiceTier.Standard, null,
            "", null, "LIC-123", "PL-123", "Honda", "Black");

        Assert.False(result.IsSuccess);
        Assert.Equal(DriverErrors.NationalIdRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenMotorcycleMissingLicenseOrPlate()
    {
        var resultNoLicense = TbDriver.Create(
            Guid.NewGuid(), Guid.NewGuid(), VehicleType.Motorcycle, ServiceTier.Standard, null,
            "12345678901", null, null, "PL-123", "Honda", "Black");
        Assert.False(resultNoLicense.IsSuccess);
        Assert.Equal(DriverErrors.LicenseNumberRequired.Code, resultNoLicense.TopError.Code);

        var resultNoPlate = TbDriver.Create(
            Guid.NewGuid(), Guid.NewGuid(), VehicleType.Motorcycle, ServiceTier.Standard, null,
            "12345678901", null, "LIC-123", null, "Honda", "Black");
        Assert.False(resultNoPlate.IsSuccess);
        Assert.Equal(DriverErrors.PlateNumberRequired.Code, resultNoPlate.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenBicycleHasPlateNumber()
    {
        var result = TbDriver.Create(
            Guid.NewGuid(), Guid.NewGuid(), VehicleType.Bicycle, ServiceTier.Standard, null,
            "12345678901", null, null, "PL-123", "Trek", "Red"); // Plate number provided for Bicycle

        Assert.False(result.IsSuccess);
        Assert.Equal(DriverErrors.PlateNumberNotAllowed.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_WhenDataIsValid()
    {
        var result = TbDriver.Create(
            Guid.NewGuid(), Guid.NewGuid(), VehicleType.Car, ServiceTier.Premium, CarType.Private,
            "12345678901", null, "LIC-123", "PL-123", "Mercedes", "Black");

        Assert.True(result.IsSuccess);
        Assert.Equal(DriverStatus.Offline, result.Value.Status); // Default
        Assert.False(result.Value.IsApproved); // Default
    }

    // ============================================================
    // State Transition Tests - Approval
    // ============================================================

    [Fact]
    public void Approve_ShouldReturnSuccess_AndSetIsApprovedToTrue()
    {
        var driver = CreateValidDriver();

        var result = driver.Approve();

        Assert.True(result.IsSuccess);
        Assert.True(driver.IsApproved);
    }

    [Fact]
    public void Approve_ShouldReturnError_WhenAlreadyApproved()
    {
        var driver = CreateValidDriver();
        driver.Approve();

        var result = driver.Approve(); // Try again

        Assert.False(result.IsSuccess);
        Assert.Equal(DriverErrors.DriverAlreadyApproved.Code, result.TopError.Code);
    }

    [Fact]
    public void Reject_ShouldReturnSuccess_AndSetIsApprovedToFalse()
    {
        var driver = CreateValidDriver();
        driver.Approve(); // Approve first

        var result = driver.Reject();

        Assert.True(result.IsSuccess);
        Assert.False(driver.IsApproved);
    }

    [Fact]
    public void Reject_ShouldReturnError_WhenAlreadyRejected()
    {
        var driver = CreateValidDriver();
        // It's already false by default

        var result = driver.Reject();

        Assert.False(result.IsSuccess);
        Assert.Equal(DriverErrors.DriverAlreadyRejected.Code, result.TopError.Code);
    }

    // ============================================================
    // State Transition Tests - Online / Offline
    // ============================================================

    [Fact]
    public void GoOnline_ShouldReturnError_WhenNotApproved()
    {
        var driver = CreateValidDriver();
        // Not approved

        var result = driver.GoOnline();

        Assert.False(result.IsSuccess);
        Assert.Equal(DriverErrors.DriverNotApproved.Code, result.TopError.Code);
    }

    [Fact]
    public void GoOnline_ShouldReturnSuccess_AndSetStatusToOnline()
    {
        var driver = CreateValidDriver();
        driver.Approve();

        var result = driver.GoOnline();

        Assert.True(result.IsSuccess);
        Assert.Equal(DriverStatus.Online, driver.Status);
    }

    [Fact]
    public void GoOnline_ShouldReturnError_WhenAlreadyOnline()
    {
        var driver = CreateValidDriver();
        driver.Approve();
        driver.GoOnline();

        var result = driver.GoOnline();

        Assert.False(result.IsSuccess);
        Assert.Equal(DriverErrors.DriverAlreadyOnline.Code, result.TopError.Code);
    }

    [Fact]
    public void GoOffline_ShouldReturnSuccess_AndSetStatusToOffline()
    {
        var driver = CreateValidDriver();
        driver.Approve();
        driver.GoOnline();

        var result = driver.GoOffline();

        Assert.True(result.IsSuccess);
        Assert.Equal(DriverStatus.Offline, driver.Status);
    }

    [Fact]
    public void GoOffline_ShouldReturnError_WhenAlreadyOffline()
    {
        var driver = CreateValidDriver();
        // Already offline by default

        var result = driver.GoOffline();

        Assert.False(result.IsSuccess);
        Assert.Equal(DriverErrors.DriverAlreadyOffline.Code, result.TopError.Code);
    }

    // ============================================================
    // State Transition Tests - Delivery Lifecycle
    // ============================================================

    [Fact]
    public void StartDelivery_ShouldReturnError_WhenNotOnline()
    {
        var driver = CreateValidDriver();
        driver.Approve();
        // Not Online yet (Offline)

        var result = driver.StartDelivery();

        Assert.False(result.IsSuccess);
        Assert.Equal(DriverErrors.DriverOffline.Code, result.TopError.Code);
    }

    [Fact]
    public void StartDelivery_ShouldReturnSuccess_AndSetStatusToOnDelivery()
    {
        var driver = CreateValidDriver();
        driver.Approve();
        driver.GoOnline();

        var result = driver.StartDelivery();

        Assert.True(result.IsSuccess);
        Assert.Equal(DriverStatus.OnDelivery, driver.Status);
    }

    [Fact]
    public void FinishDelivery_ShouldReturnError_WhenNotOnDelivery()
    {
        var driver = CreateValidDriver();
        driver.Approve();
        driver.GoOnline(); // Online but not OnDelivery

        var result = driver.FinishDelivery();

        Assert.False(result.IsSuccess);
        Assert.Equal(DriverErrors.DriverOnDelivery.Code, result.TopError.Code);
    }

    [Fact]
    public void FinishDelivery_ShouldReturnSuccess_AndSetStatusBackToOnline()
    {
        var driver = CreateValidDriver();
        driver.Approve();
        driver.GoOnline();
        driver.StartDelivery();

        var result = driver.FinishDelivery();

        Assert.True(result.IsSuccess);
        Assert.Equal(DriverStatus.Online, driver.Status);
    }

    // ============================================================
    // State Transition Tests - Restrictions While On Delivery
    // ============================================================

    [Fact]
    public void Actions_ShouldReturnError_WhenDriverIsOnDelivery()
    {
        var driver = CreateValidDriver();
        driver.Approve();
        driver.GoOnline();
        driver.StartDelivery();

        // Try to go offline
        var resultOffline = driver.GoOffline();
        Assert.False(resultOffline.IsSuccess);
        Assert.Equal(DriverErrors.CannotChangeStatusWhileOnDelivery.Code, resultOffline.TopError.Code);

        // Try to stop
        var resultStop = driver.Stop();
        Assert.False(resultStop.IsSuccess);
        Assert.Equal(DriverErrors.CannotChangeStatusWhileOnDelivery.Code, resultStop.TopError.Code);

        // Try to update vehicle
        var resultVehicle = driver.UpdateVehicle(VehicleType.Car, ServiceTier.Standard, CarType.Private, "LIC-NEW", "PL-NEW", "Toyota", "White");
        Assert.False(resultVehicle.IsSuccess);
        Assert.Equal(DriverErrors.CannotChangeVehicleWhileOnDelivery.Code, resultVehicle.TopError.Code);
    }

    // ============================================================
    // Update Tests
    // ============================================================

    [Fact]
    public void UpdateVehicle_ShouldReturnSuccess_AndUpdateProperties()
    {
        var driver = CreateValidDriver(); // Default Motorcycle
        var newCarType = CarType.Taxi;

        var result = driver.UpdateVehicle(VehicleType.Car, ServiceTier.Standard, newCarType, "LIC-CAR", "PL-CAR", "Toyota", "White");

        Assert.True(result.IsSuccess);
        Assert.Equal(VehicleType.Car, driver.VehicleType);
        Assert.Equal(newCarType, driver.CarType);
        Assert.Equal("LIC-CAR", driver.LicenseNumber);
        Assert.Equal("PL-CAR", driver.PlateNumber);
    }

    [Fact]
    public void UpdatePhoto_ShouldReturnError_WhenUrlIsTooLong()
    {
        var driver = CreateValidDriver();

        var result = driver.UpdatePhoto(new string('a', 1001));

        Assert.False(result.IsSuccess);
        Assert.Equal(DriverErrors.PhotoUrlTooLong.Code, result.TopError.Code);
    }

    [Fact]
    public void UpdatePhoto_ShouldReturnSuccess_AndUpdateProperty()
    {
        var driver = CreateValidDriver();
        var newPhoto = "https://example.com/new-photo.jpg";

        var result = driver.UpdatePhoto(newPhoto);

        Assert.True(result.IsSuccess);
        Assert.Equal(newPhoto, driver.PhotoUrl);
    }
}
