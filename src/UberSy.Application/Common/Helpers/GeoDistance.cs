using NetTopologySuite.Geometries;

namespace UberSy.Application.Common.Helpers;


public static class GeoDistance
{
    public static double HaversineKm(Point a, Point b)
    {
        const double earthRadiusKm = 6371.0;

        var dLat = ToRadians(b.Y - a.Y);
        var dLon = ToRadians(b.X - a.X);

        var h = Math.Pow(Math.Sin(dLat / 2), 2) +
                Math.Cos(ToRadians(a.Y)) * Math.Cos(ToRadians(b.Y)) * Math.Pow(Math.Sin(dLon / 2), 2);

        return 2 * earthRadiusKm * Math.Asin(Math.Sqrt(h));
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180;
}