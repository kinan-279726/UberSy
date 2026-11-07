using NetTopologySuite.Geometries;
using UberSy.Domain.Common.Results;
using UberSy.Domain.ServiceZones;

namespace UberSy.Test.Common.ServiceZone;

public static class ServiceZoneFactory
{
    public static Result<TbServiceZone> CreateServiceZone(
        Guid? id = null,
        string? nameAr = null,
        string? nameEn = null,
        Geometry? boundary = null,
        string? city = null)
    {
        return TbServiceZone.Create(
            id ?? Guid.NewGuid(),
            nameAr ?? "منطقة دمشق المركزية",
            nameEn ?? "Damascus Central Zone",
            boundary ?? CreatePolygon(),
            city ?? "Damascus");
    }

    private static Polygon CreatePolygon()
    {
        var coords = new[]
        {
            new Coordinate(36.27, 33.51),
            new Coordinate(36.29, 33.51),
            new Coordinate(36.29, 33.53),
            new Coordinate(36.27, 33.53),
            new Coordinate(36.27, 33.51),
        };

        var ring = new LinearRing(coords);
        return new Polygon(ring) { SRID = 4326 };
    }
}
