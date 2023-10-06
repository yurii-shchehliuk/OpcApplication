using Microsoft.AspNetCore.SignalR;
using Qia.Opc.Domain.Common;
using Qia.Opc.Domain.Core;

namespace Qia.Opc.Infrastrucutre.Services.Communication
{
	public class SignalRService
	{
		private readonly IHubContext<ChatHub> _hubConnection;

		public SignalRService(IHubContext<ChatHub> _hubContext)
		{
			_hubConnection = _hubContext;
		}

		public async Task SendNodeAction(object nodeData, string groupName)
		{
			await _hubConnection.Clients.Group(groupName).SendAsync("SendNodeAction", nodeData);
		}
	}
}
