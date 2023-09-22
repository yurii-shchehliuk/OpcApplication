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
		public async Task SendSettingsAction(string groupName, object settings)
		{
			if (string.IsNullOrEmpty(groupName) || groupName == "All")
			{
				await Clients.All.SendAsync("SettingsReceived", settings);
			}
			else
			{
				await Clients.Group(groupName).SendAsync("SettingsReceived", settings);
			}
		}

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

		public async Task SendConfigNodesAction(string groupName, object nodeData)
		{
			if (string.IsNullOrEmpty(groupName) || groupName == "All")
			{
				await Clients.All.SendAsync("ConfigNodesReceived", nodeData);
			}
			else
			{
				await Clients.Group(groupName).SendAsync("ConfigNodesReceived", nodeData);
			}
		}

		public async Task SendGraphAction(string groupName, string nodesTree)
		{
			if (string.IsNullOrEmpty(groupName) || groupName == "All")
			{
				await Clients.All.SendAsync("GraphReceived", groupName, nodesTree);
			}
			else
			{
				await Clients.Group(groupName).SendAsync("GraphReceived", groupName, nodesTree);
			}
		}

		public async Task SendGroupsAction(object groupsArr)
		{
			await Clients.All.SendAsync("GroupsReceived", groupsArr);
		}
		#endregion

		#region Call from frontend to opc client
		///<remarks>
		///The idea is that the frontend client is communicating with the particular Group.
		///Whereas the OPC client accepts everything as All and sends it back to the selected Group. 
		///The idea is based on the assumption that the OPC client probably shouldn't be joined in all the groups.
		///</remarks>
		public async Task JoinGroup(string groupName)
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
			Console.WriteLine($"{Context.ConnectionId} joined {groupName}");
		}

		public async Task RemoveFromGroup(string groupName)
		{
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
		}

		public async Task GetGraphWeb(string groupName, bool isFullGraph)
		{
			await Clients.All.SendAsync("GetGraphWeb", groupName, isFullGraph);
		}

		public async Task NodeMonitorWeb(object node)
		{
			await Clients.All.SendAsync("NodeMonitorWeb", node);
		}

		public async Task GetNodesWeb(string groupName)
		{
			await Clients.All.SendAsync("GetNodesWeb", groupName);
		}

		public async Task SaveSettingsWeb(string groupName, object settings)
		{
			await Clients.All.SendAsync("SaveSettingsWeb", groupName, settings);
		}

		public async Task GetSettingsWeb(string groupName)
		{
			await Clients.All.SendAsync("GetSettingsWeb", groupName);
		}

		public async Task GetGroupsWeb()
		{
			await Clients.All.SendAsync("GetGroupsWeb");
		}

		public async Task RemoveGroupWeb(string groupName)
		{
			await Clients.All.SendAsync("RemoveGroupWeb", groupName);
		}

		public async Task AddGroupWeb(string groupName)
		{
			await Clients.All.SendAsync("AddGroupWeb", groupName);
		}
		#endregion
	}
}
