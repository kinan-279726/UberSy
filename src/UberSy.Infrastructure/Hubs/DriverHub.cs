using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using UberSy.Application.Features.Drivers.Commands.UpdateDriverLocation;

namespace UberSy.Infrastructure.Hubs;

//[Authorize(Roles = "driver")]
public sealed class DriverHub(IMediator mediator) : Hub
{
    private readonly IMediator _mediator = mediator;

    public async Task JoinDriverGroup(Guid driverId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GetGroupName(driverId));
    }

    public async Task LeaveDriverGroup(Guid driverId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetGroupName(driverId));
    }

    public async Task UpdateLocation(Guid driverId, double latitude, double longitude)
    {
        var command = new UpdateDriverLocationCommand(driverId, latitude, longitude);
        var result = await _mediator.Send(command, Context.ConnectionAborted);

        if (result.IsError)
        {
            await Clients.Caller.SendAsync("LocationUpdateFailed", new
            {
                result.TopError.Code,
                result.TopError.Description,
            });
        }
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"driver {driverId} is sending hes location ( latitude: {latitude} , longitude: {longitude} )");
        Console.ResetColor();
    }

    public static string GetGroupName(Guid driverId) => $"driver-{driverId}";
}
