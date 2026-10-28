using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using System.Net.Http.Json;
using UberSy.Application.Common.Helpers;
using UberSy.Application.Common.Interfaces;

namespace UberSy.Infrastructure.Services;

public sealed class OsrmRoutingService(
    HttpClient http,
    HybridCache cache,
    ILogger<OsrmRoutingService> logger) : IRoutingService
{
    private readonly HttpClient _http = http;
    private readonly HybridCache _cache = cache;
    private readonly ILogger<OsrmRoutingService> _logger = logger;

    public async Task<RouteResult> GetDistanceAsync(Point from, Point to, CancellationToken ct = default)
    {
        var key = FormattableString.Invariant($"route:{from.X:F4},{from.Y:F4}:{to.X:F4},{to.Y:F4}");

        var result = await _cache.GetOrCreateAsync(key, async token => await FetchAsync(from, to, token), cancellationToken: ct);

        if (result.IsApproximate)
        {
            await _cache.RemoveAsync(key, ct);
        }

        return result;
    }

    private async ValueTask<RouteResult> FetchAsync(Point from, Point to, CancellationToken ct)
    {
        try
        {
            var url = FormattableString.Invariant(
                $"route/v1/driving/{from.X},{from.Y};{to.X},{to.Y}?overview=false");

            var response = await _http.GetFromJsonAsync<OsrmResponse>(url, ct);
            var meters = response?.Routes?.FirstOrDefault()?.Distance;

            if (response?.Code == "Ok" && meters is not null)
            {
                return new RouteResult(Math.Round((decimal)meters.Value / 1000m, 2), IsApproximate: false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "OSRM Request Failed, Falling Back To Straight-Line Estimate.");
        }

        var approx = Math.Round((decimal)GeoDistance.HaversineKm(from, to) * 1.3m, 2);
        return new RouteResult(approx, IsApproximate: true);
    }

    private sealed record OsrmResponse(string Code, List<OsrmRoute>? Routes);
    private sealed record OsrmRoute(double Distance, double Duration);
}