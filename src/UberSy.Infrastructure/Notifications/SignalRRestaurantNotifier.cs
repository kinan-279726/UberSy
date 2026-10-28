using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Identity;
using UberSy.Infrastructure.Hubs;

namespace UberSy.Infrastructure.Notifications;

[Authorize(Roles = Roles.Owner)]
public sealed class SignalRRestaurantNotifier(IHubContext<RestaurantHub> hubContext ) : IRestaurantNotifier
{
    private readonly IHubContext<RestaurantHub> _hubContext = hubContext;

    public async Task NotifyNewOrderAsync(Guid restaurantId, Guid serviceRequestId, CancellationToken ct = default)
    {
        var groupName = RestaurantHub.GetGroupName(restaurantId);

        await _hubContext.Clients.Group(groupName).SendAsync(
            "NewOrderReceived",
            new { ServiceRequestId = serviceRequestId, ReceivedAtUtc = DateTime.UtcNow },
            ct);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"resturant with id {restaurantId} has been notifyerd with new id");
        Console.ResetColor();
    }

    public async Task NotifyOrderCancledAsync(Guid restaurantId, Guid serviceRequestId, CancellationToken ct = default)
    {
        var groupName = RestaurantHub.GetGroupName(restaurantId);

        await _hubContext.Clients.Group(groupName).SendAsync(
            "OrderCancledByUser",
            new { ServiceRequestId = serviceRequestId, ReceivedAtUtc = DateTime.UtcNow },
            ct);
    }
}
