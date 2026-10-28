using Microsoft.AspNetCore.SignalR;

namespace UberSy.Infrastructure.Hubs;

public sealed class UserHub : Hub
{
    public async Task JoinUserGroup(Guid userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GetGroupName(userId));
    }

    public async Task LeaveUserGroup(Guid userId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetGroupName(userId));
    }
    public static string GetGroupName(Guid userId) => $"user-{userId}";
}
