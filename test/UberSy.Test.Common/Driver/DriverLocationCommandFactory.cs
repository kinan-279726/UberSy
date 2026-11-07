using UberSy.Application.Features.Drivers.Commands.UpdateDriverLocation;

namespace UberSy.Test.Common.Driver;

public static class DriverLocationCommandFactory
{
    public static UpdateDriverLocationCommand CreateUpdateDriverLocationCommand(
        Guid? driverId = null,
        double? latitude = null,
        double? longitude = null)
    {
        return new UpdateDriverLocationCommand(
            driverId ?? Guid.NewGuid(),
            latitude ?? 33.5138,   // Damascus Latitude
            longitude ?? 36.2765); // Damascus Longitude
    }
}
