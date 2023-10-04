using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using System.Xml.Linq;

namespace WebApplication1.Services
{
	public class ChatHub : Hub
	{
		public static string connectionId = "";

		public override Task OnConnectedAsync()
		{
			var context = Context.GetHttpContext();
			connectionId = Context.ConnectionId;

			Console.WriteLine($"# Connected from: {context?.Connection?.RemoteIpAddress?.ToString()}");

			return base.OnConnectedAsync();
		}

		#region opc client sends to frontend

		public async Task SendNodeAction(string groupName, object nodeData)
		{
			if (string.IsNullOrEmpty(groupName) || groupName == "All")
			{
				await Clients.All.SendAsync("NodeReceived", nodeData);
			}
			else
			{
				await Clients.Group(groupName).SendAsync("NodeReceived", nodeData);
			}
		}
		#endregion

		///<remarks>
		///The idea is that the frontend client is communicating with the particular Group.
		///</remarks>
		public async Task JoinGroup(string groupName)
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
			Console.WriteLine($"{Context.ConnectionId} joined {groupName}");
		}

		public async Task LeaveGroup(string groupName)
		{
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
		}
	}
}
