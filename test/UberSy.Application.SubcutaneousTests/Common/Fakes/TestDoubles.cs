using System.Collections.Concurrent;
using NetTopologySuite.Geometries;
using UberSy.Application.Common.Interfaces;
using Xunit;

namespace UberSy.Application.SubcutaneousTests.Common.Fakes;

/// <summary>One recorded notification (who was notified, about which service request / offer).</summary>
public sealed record NotificationRecord(string Method, Guid RecipientId, Guid ServiceRequestId, Guid? OfferId = null);

/// <summary>
/// Replaces the three SignalR notifiers. It records every call so tests can assert on notifications
/// without needing a hub. Register ONE singleton instance for IUserNotifier, IDriverNotifier and IRestaurantNotifier.
/// </summary>
public sealed class FakeNotifier : IUserNotifier, IDriverNotifier, IRestaurantNotifier
{
    private readonly ConcurrentQueue<NotificationRecord> _records = new();

    public IReadOnlyList<NotificationRecord> Records => _records.ToArray();

    public void Clear()
    {
        while (_records.TryDequeue(out _))
        {
        }
    }

    public IReadOnlyList<NotificationRecord> Of(string method)
        => _records.Where(r => r.Method == method).ToArray();

    private Task Record(string method, Guid recipientId, Guid serviceRequestId, Guid? offerId = null)
    {
        _records.Enqueue(new NotificationRecord(method, recipientId, serviceRequestId, offerId));
        return Task.CompletedTask;
    }

    // ---------- IUserNotifier ----------
    public Task NotifyServiceOutForDelivery(Guid serviceRequestId, Guid customerId, CancellationToken cancellationToken = default)
        => Record(nameof(IUserNotifier.NotifyServiceOutForDelivery), customerId, serviceRequestId);

    public Task NotifyServiceCancelledRestaurant(Guid serviceRequestId, Guid customerId, CancellationToken cancellationToken = default)
        => Record(nameof(IUserNotifier.NotifyServiceCancelledRestaurant), customerId, serviceRequestId);

    public Task NotifyServiceAcceptedByRestaurant(Guid serviceRequestId, Guid customerId, CancellationToken cancellationToken = default)
        => Record(nameof(IUserNotifier.NotifyServiceAcceptedByRestaurant), customerId, serviceRequestId);

    public Task NotifyServiceCancelledBySystem(Guid serviceRequestId, Guid customerId, CancellationToken cancellationToken = default)
        => Record(nameof(IUserNotifier.NotifyServiceCancelledBySystem), customerId, serviceRequestId);

    public Task NotifyServiceDelivered(Guid serviceRequestId, Guid customerId, CancellationToken cancellationToken = default)
        => Record(nameof(IUserNotifier.NotifyServiceDelivered), customerId, serviceRequestId);

    public Task NotifyDriverAssigned(Guid serviceRequestId, Guid customerId, CancellationToken cancellationToken = default)
        => Record(nameof(IUserNotifier.NotifyDriverAssigned), customerId, serviceRequestId);

    public Task NotifyDriverLocationAsync(Guid serviceRequestId, Guid customerId, double latitude, double longitude, CancellationToken cancellationToken = default)
        => Record(nameof(IUserNotifier.NotifyDriverLocationAsync), customerId, serviceRequestId);

    // ---------- IDriverNotifier ----------
    public Task NotifyOfferAsync(Guid driverId, Guid offerId, Guid serviceRequestId, CancellationToken cancellation = default)
        => Record(nameof(IDriverNotifier.NotifyOfferAsync), driverId, serviceRequestId, offerId);

    public Task NotifyOfferLostAsync(Guid driverId, Guid offerId, Guid serviceRequestId, CancellationToken cancellation = default)
        => Record(nameof(IDriverNotifier.NotifyOfferLostAsync), driverId, serviceRequestId, offerId);

    public Task NotifyOrderCanceldAsync(Guid driverId, CancellationToken cancellation = default)
        => Record(nameof(IDriverNotifier.NotifyOrderCanceldAsync), driverId, Guid.Empty);

    // ---------- IRestaurantNotifier ----------
    public Task NotifyNewOrderAsync(Guid restaurantId, Guid serviceRequestId, CancellationToken ct = default)
        => Record(nameof(IRestaurantNotifier.NotifyNewOrderAsync), restaurantId, serviceRequestId);

    public Task NotifyOrderCancledAsync(Guid restaurantId, Guid serviceRequestId, CancellationToken ct = default)
        => Record(nameof(IRestaurantNotifier.NotifyOrderCancledAsync), restaurantId, serviceRequestId);
}

/// <summary>Replaces the OSRM routing client: returns a fixed, configurable distance (no network calls).</summary>
public sealed class FakeRoutingService : IRoutingService
{
    public const decimal DefaultDistanceKm = 3m;

    public decimal DistanceKm { get; set; } = DefaultDistanceKm;

    public void Reset() => DistanceKm = DefaultDistanceKm;

    public Task<RouteResult> GetDistanceAsync(Point from, Point to, CancellationToken ct = default)
        => Task.FromResult(new RouteResult(DistanceKm, IsApproximate: false));
}

/// <summary>Configuration values pinned for the tests (they override appsettings.json through UseSetting).</summary>
public static class TestSettings
{
    public const decimal CustomerRatePerKm = 2500m;
    public const decimal DriverRatePerKm = 2000m;
    public const decimal MaxDistanceKm = 15m;
    public const int DispatchBatchSize = 2;
    public const int OfferTimeoutMinutes = 1;
    public const int RestaurantResponseTimeoutMinutes = 10;

    public static IReadOnlyDictionary<string, string> All { get; } = new Dictionary<string, string>
    {
        ["DeliveryPricing:Default:CustomerRatePerKm"] = CustomerRatePerKm.ToString(System.Globalization.CultureInfo.InvariantCulture),
        ["DeliveryPricing:Default:DriverRatePerKm"] = DriverRatePerKm.ToString(System.Globalization.CultureInfo.InvariantCulture),
        ["DeliveryPricing:Default:MaxDistanceKm"] = MaxDistanceKm.ToString(System.Globalization.CultureInfo.InvariantCulture),
        ["DispatchSettings:BatchSize"] = DispatchBatchSize.ToString(),
        ["DispatchSettings:OfferTimeoutMinutes"] = OfferTimeoutMinutes.ToString(),
        ["OrderSettings:RestaurantResponseTimeoutMinutes"] = RestaurantResponseTimeoutMinutes.ToString(),
    };
}
