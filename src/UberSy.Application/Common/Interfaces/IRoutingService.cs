using NetTopologySuite.Geometries;

namespace UberSy.Application.Common.Interfaces;
public sealed record RouteResult(decimal DistanceKm, bool IsApproximate);

public interface IRoutingService
{
    Task<RouteResult> GetDistanceAsync(Point from, Point to, CancellationToken ct = default);
}