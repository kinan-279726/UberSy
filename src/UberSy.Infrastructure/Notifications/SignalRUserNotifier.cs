using Microsoft.AspNetCore.SignalR;
using UberSy.Application.Common.Interfaces;
using UberSy.Infrastructure.Hubs;

namespace UberSy.Infrastructure.Notifications;

public sealed class SignalRUserNotifier(IHubContext<UserHub> hubContext) : IUserNotifier
{
    private readonly IHubContext<UserHub> _hubContext = hubContext;

    // preparing
    public async Task NotifyServiceAcceptedByRestaurant(Guid serviceRequestId, Guid customerId, CancellationToken cancellationToken = default)
    {
        var groupName = UserHub.GetGroupName(customerId);

        await _hubContext.Clients.Group(groupName).SendAsync(
            "ServiceAcceptedByRestaurant",
            new { ServiceRequestId = serviceRequestId, CustomerId = customerId, ReceivedAtUtc = DateTimeOffset.UtcNow },
            cancellationToken);
    }

    public async Task NotifyServiceCancelledRestaurant(Guid serviceRequestId, Guid customerId, CancellationToken cancellationToken = default)
    {
        var groupName = UserHub.GetGroupName(customerId);

        await _hubContext.Clients.Group(groupName).SendAsync(
            "ServiceCancelledByRestaurant",
            new { ServiceRequestId = serviceRequestId, CustomerId = customerId, ReceivedAtUtc = DateTimeOffset.UtcNow },
            cancellationToken);
    }

    public async Task NotifyServiceCancelledBySystem(Guid serviceRequestId, Guid customerId, CancellationToken cancellationToken = default)
    {
        var groupName = UserHub.GetGroupName(customerId);

        await _hubContext.Clients.Group(groupName).SendAsync(
            "ServiceCancelledBySystem",
            new { ServiceRequestId = serviceRequestId, CustomerId = customerId, ReceivedAtUtc = DateTimeOffset.UtcNow },
            cancellationToken);
    }

    public async Task NotifyServiceOutForDelivery(Guid serviceRequestId, Guid customerId, CancellationToken cancellationToken = default)
    {
        var groupName = UserHub.GetGroupName(customerId);

        await _hubContext.Clients.Group(groupName).SendAsync(
            "ServiceOutForDelivery",
            new { ServiceRequestId = serviceRequestId, CustomerId = customerId, ReceivedAtUtc = DateTimeOffset.UtcNow },
            cancellationToken);
    }

    public async Task NotifyServiceDelivered(Guid serviceRequestId, Guid customerId, CancellationToken cancellationToken = default)
    {
        var groupName = UserHub.GetGroupName(customerId);

        await _hubContext.Clients.Group(groupName).SendAsync(
            "ServiceDelivered",
            new { ServiceRequestId = serviceRequestId, CustomerId = customerId, ReceivedAtUtc = DateTimeOffset.UtcNow },
            cancellationToken);
    }

    public async Task NotifyDriverAssigned(Guid serviceRequestId, Guid customerId, CancellationToken cancellationToken = default)
    {
        var groupName = UserHub.GetGroupName(customerId);

        await _hubContext.Clients.Group(groupName).SendAsync(
            "DriverAssigned",
            new { ServiceRequestId = serviceRequestId, CustomerId = customerId, ReceivedAtUtc = DateTimeOffset.UtcNow },
            cancellationToken);
    }

    public async Task NotifyDriverLocationAsync(Guid serviceRequestId, Guid customerId,
            double latitude, double longitude, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(UserHub.GetGroupName(customerId)).SendAsync(
            "DriverLocationUpdated",
            new
            {
                ServiceRequestId = serviceRequestId,
                Latitude = latitude,
                Longitude = longitude,
                ReceivedAtUtc = DateTimeOffset.UtcNow
            },
            cancellationToken);
    }
}
