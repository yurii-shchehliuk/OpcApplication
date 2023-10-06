using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Qia.Opc.Infrastrucutre.Services.Communication
{
	public class ChatHub : Hub
	{
		public static string connectionId = "";

		public override Task OnConnectedAsync()
		{
			var httpContext = Context.Features.Get<IHttpContextFeature>();
			var connectionId = Context.ConnectionId; 

			Console.WriteLine($"# Connected {connectionId} from: {httpContext?.HttpContext?.Connection?.RemoteIpAddress?.ToString()}");

			return base.OnConnectedAsync();
		}

		#region frontend calls to server

		public async Task GetConnectionId()
		{
			await Clients.Caller.SendAsync("ReceiveConnectionId", Context.ConnectionId);
		}

		public async Task JoinGroup(string groupName)
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
			Console.WriteLine($"{Context.ConnectionId} joined {groupName}");
		}

		public async Task LeaveGroup(string groupName)
		{
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
		}
		#endregion
	}
}
