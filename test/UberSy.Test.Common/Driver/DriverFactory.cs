using UberSy.Domain.Common.Results;
using UberSy.Domain.DeliveryRule.Enums;
using UberSy.Domain.Drivers;

namespace UberSy.Test.Common.Driver;

public static class DriverFactory
{
    public static Result<TbDriver> CreateDriver(
        Guid? id = null,
        Guid? userId = null,
        VehicleType? vehicleType = null,
        ServiceTier? serviceTier = null,
        CarType? carType = null,
        string? nationalId = null,
        string? photoUrl = null,
        string? licenseNumber = null,
        string? plateNumber = null,
        string? model = null,
        string? color = null)
    {
        var vt = vehicleType ?? VehicleType.Motorcycle;

        return TbDriver.Create(
            id ?? Guid.NewGuid(),
            userId ?? Guid.NewGuid(),
            vt,
            serviceTier ?? ServiceTier.Standard,
            carType,
            nationalId ?? "0123456789",
            photoUrl,
            licenseNumber ?? (vt == VehicleType.Bicycle ? null : "LIC-12345"),
            plateNumber ?? (vt == VehicleType.Bicycle ? null : "PLT-9876"),
            model,
            color);
    }

    public static TbDriver CreateValidDriver(
    Guid? userId = null,
    bool approved = false,
    bool online = false,
    string? nationalId = null)
    {
        var result = CreateDriver(userId: userId, nationalId: nationalId);

        if (result.IsError)
            throw new InvalidOperationException($"{result.TopError.Code}: {result.TopError.Description}");

        var driver = result.Value;

        if (approved) driver.Approve();
        if (online) driver.GoOnline();

        return driver;
    }
}
