using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.AspNetCore.SignalR;
using SignalR.Hub.Entities;
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

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            Console.WriteLine($"{Context.ConnectionId} joined {groupName}");
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task SendMessage(string groupName, string nodeData)
        {
            if (string.IsNullOrEmpty(groupName) || groupName == "All")
            {
                await Clients.All.SendAsync("MessageReceived", nodeData);
            }
            else
            {
                await Clients.Group(groupName).SendAsync("MessageReceivedGroup", nodeData);
            }
        }
        /// <summary>
        /// opc client sends to frontend
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="graphName"></param>
        /// <param name="nodesTree"></param>
        /// <returns></returns>
        public async Task SendGraphAction(string groupName, string graphName, string nodesTree)
        {
            if (string.IsNullOrEmpty(groupName) || groupName == "All")
            {
                await Clients.All.SendAsync("LoadGraph", groupName, graphName, nodesTree);
            }
            else
            {
                await Clients.Group(groupName).SendAsync("LoadGraph", groupName, graphName, nodesTree);
            }
        }
        /// <summary>
        /// Call from frontend to get graph from opc client
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="graphName"></param>
        /// <returns></returns>
        public async Task GetGraphAction(string groupName, string graphName)
        {
            if (string.IsNullOrEmpty(groupName) || groupName == "All")
            {
                await Clients.All.SendAsync("GetGraph", groupName, graphName);
            }
            else
            {
                await Clients.Group(groupName).SendAsync("GetGraph", groupName, graphName);
            }
        }

        public async Task NodeMonitorAction(NewNode node)
        {
            if (string.IsNullOrEmpty(node.Group) || node.Group == "All")
            {
                await Clients.All.SendAsync("NodeMonitor", node);
            }
            else
            {
                await Clients.Group(node.Group).SendAsync("NodeMonitor", node);
            }
            Console.WriteLine($"Connection: {Context.ConnectionId} nodeId: {node.NodeId} group: {node.Group} action: {node.Action}");
        }
    }
}
