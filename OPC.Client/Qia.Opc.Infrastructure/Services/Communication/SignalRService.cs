namespace QIA.Opc.Infrastructure.Services.Communication;

using Microsoft.AspNetCore.SignalR;

public class SignalRService
{
    private readonly IHubContext<ChatHub> _hubConnection;

    public SignalRService(IHubContext<ChatHub> HubContext)
    {
        _hubConnection = HubContext;
    }

    public async Task SendNodeAsync(object nodeData, string groupName) => await _hubConnection.Clients.Group(groupName).SendAsync("SendNodeAction", nodeData);

    public async Task SendSubscriptionAsync(object subscriptionData, string groupName) => await _hubConnection.Clients.Group(groupName).SendAsync("SendSubscriptionAction", subscriptionData);

    public async Task SendNotification(Domain.Entities.NotificationData e, string sessionId) => await _hubConnection.Clients.Group(sessionId).SendAsync("SendNotificationAction", e);
}
