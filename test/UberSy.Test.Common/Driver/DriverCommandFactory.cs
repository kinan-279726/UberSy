using UberSy.Application.Features.Drivers.Commands.ApproveDriver;
using UberSy.Application.Features.Drivers.Commands.GoOffline;
using UberSy.Application.Features.Drivers.Commands.GoOnline;
using UberSy.Application.Features.Drivers.Commands.RegisterDriver;
using UberSy.Domain.DeliveryRule.Enums;

namespace UberSy.Test.Common.Driver;

public static class DriverCommandFactory
{
    // 1. Approve Driver
    public static ApproveDriverCommand CreateApproveDriverCommand(
        Guid? driverId = null,
        ServiceTier? serviceTier = null)
    {
        return new ApproveDriverCommand(
            driverId ?? Guid.NewGuid(),
            serviceTier ?? ServiceTier.Standard);
    }

    // 2. Go Offline (No Parameters)
    public static GoOfflineCommand CreateGoOfflineCommand()
    {
        return new GoOfflineCommand();
    }

    // 3. Go Online (No Parameters)
    public static GoOnlineCommand CreateGoOnlineCommand()
    {
        return new GoOnlineCommand();
    }

    // 4. Register Driver
    public static RegisterDriverCommand CreateRegisterDriverCommand(
        VehicleType? vehicleType = null,
        CarType? carType = null,
        string? nationalId = null,
        string? photoUrl = null,
        string? licenseNumber = null,
        string? plateNumber = null,
        string? model = null,
        string? color = null)
    {
        return new RegisterDriverCommand(
            vehicleType ?? VehicleType.Motorcycle,
            carType,
            nationalId ?? "0123456789",
            photoUrl,
            licenseNumber ?? "LIC-12345",
            plateNumber ?? "PLT-9876",
            model,
            color);
    }
}
