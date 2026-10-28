using Microsoft.AspNetCore.SignalR;

namespace UberSy.Infrastructure.Hubs;

public sealed class RestaurantHub : Hub
{
    public async Task JoinRestaurantGroup(Guid restaurantId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GetGroupName(restaurantId));
    }

    public async Task LeaveRestaurantGroup(Guid restaurantId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetGroupName(restaurantId));
    }

    public static string GetGroupName(Guid restaurantId) => $"restaurant-{restaurantId}";
}