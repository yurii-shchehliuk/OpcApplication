using Microsoft.AspNetCore.SignalR;
using Qia.Opc.Domain.Common;
using Qia.Opc.Domain.Core;

namespace QIA.Opc.Infrastructure.Services.Communication
{
	public class SignalRService
	{
		private readonly IHubContext<ChatHub> _hubConnection;

		public SignalRService(IHubContext<ChatHub> _hubContext)
		{
			_hubConnection = _hubContext;
		}

		public async Task SendNodeAsync(object nodeData, string groupName)
		{
			await _hubConnection.Clients.Group(groupName).SendAsync("SendNodeAction", nodeData);
		}

		public async Task SendSubscriptionAsync(object subscriptionData, string groupName)
		{
			await _hubConnection.Clients.Group(groupName).SendAsync("SendSubscriptionAction", subscriptionData);
		}

		public async Task SendEventMessageAsync(object eventData, string groupName)
		{
			await _hubConnection.Clients.Group(groupName).SendAsync("SendEventMessageAction", eventData);
		}
	}
}
