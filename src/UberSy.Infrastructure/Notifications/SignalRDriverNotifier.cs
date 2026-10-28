using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Identity;
using UberSy.Infrastructure.Hubs;

namespace UberSy.Infrastructure.Notifications;

[Authorize(Roles = Roles.Driver)]
public class SignalRDriverNotifier(IHubContext<DriverHub> hubContext) : IDriverNotifier
{
    private readonly IHubContext<DriverHub> _hubContext = hubContext;

    public async Task NotifyOfferAsync(Guid driverId, Guid offerId, Guid serviceRequestId, CancellationToken cancellation = default)
    {
        var groupName = DriverHub.GetGroupName(driverId);

        await _hubContext.Clients.Group(groupName).SendAsync(
            "NewOfferReceived", new { OfferId = offerId, ServiceRequestId = serviceRequestId, ReceivedAtUtc = DateTimeOffset.UtcNow },
           cancellation);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"driver with id {driverId} notify new order");
        Console.ResetColor();
    }

    public async Task NotifyOfferLostAsync(Guid driverId, Guid offerId, Guid serviceRequestId, CancellationToken cancellation = default)
    {
        var groupName = DriverHub.GetGroupName(driverId);

        await _hubContext.Clients.Group(groupName).SendAsync(
            "OfferLostToAnotherDriver", new { OfferId = offerId, ServiceRequestId = serviceRequestId, ReceivedAtUtc = DateTimeOffset.UtcNow });

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"driver with id {driverId} lose new order");
        Console.ResetColor();
    }

    public async Task NotifyOrderCanceldAsync(Guid driverId, CancellationToken cancellation = default)
    {
        var groupName = DriverHub.GetGroupName(driverId);

        await _hubContext.Clients.Group(groupName).SendAsync(
            "OrderCanceld", new { ReceivedAtUtc = DateTimeOffset.UtcNow });
    }
}
