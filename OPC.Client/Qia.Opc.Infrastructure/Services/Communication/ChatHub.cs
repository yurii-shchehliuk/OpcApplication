namespace QIA.Opc.Infrastructure.Services.Communication;

using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.SignalR;
using QIA.Opc.Infrastructure.Application;

public class ChatHub : Hub
{
    public override Task OnConnectedAsync()
    {
        IHttpContextFeature httpContext = Context.Features.Get<IHttpContextFeature>();
        var connectionId = Context.ConnectionId;

        LoggerManager.Logger.Information($"# Connected {connectionId} from: {httpContext?.HttpContext?.Connection?.RemoteIpAddress?.ToString()}");

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        IHttpContextFeature httpContext = Context.Features.Get<IHttpContextFeature>();
        var connectionId = Context.ConnectionId;

        LoggerManager.Logger.Information($"# Disconnected {connectionId} from: {httpContext?.HttpContext?.Connection?.RemoteIpAddress?.ToString()}");
        return base.OnDisconnectedAsync(exception);
    }

    #region frontend calls to server

    public async Task GetConnectionId() => await Clients.Caller.SendAsync("ReceiveConnectionId", Context.ConnectionId);

    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        LoggerManager.Logger.Information($"{Context.ConnectionId} joined {groupName}");
    }

    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        LoggerManager.Logger.Information($"{Context.ConnectionId} leaved {groupName}");
    }
    #endregion
}
