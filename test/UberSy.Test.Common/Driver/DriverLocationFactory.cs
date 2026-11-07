using NetTopologySuite.Geometries;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Drivers;

namespace UberSy.Test.Common.Driver;

public static class DriverLocationFactory
{
    public static Result<TbDriverLocation> CreateDriverLocation(
        Guid? id = null,
        Guid? driverId = null,
        Point? boundary = null)
    {
        return TbDriverLocation.Create(
            id ?? Guid.NewGuid(),
            driverId ?? Guid.NewGuid(),
            boundary ?? new Point(36.2765, 33.5138) { SRID = 4326 });
    }
}
