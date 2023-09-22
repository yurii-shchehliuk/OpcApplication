using AutoMapper;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Azure.Amqp.Encoding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using QIA.Plugin.OpcClient.Core;
using QIA.Plugin.OpcClient.Database;
using QIA.Plugin.OpcClient.Entities;
using QIA.Plugin.OpcClient.Repository;
using QIA.Plugin.OpcClient.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace QIA.Plugin.OpcClient.Services
{
	public class SignalRService
	{
		private readonly HubConnection _hubConnection;
		private readonly OpcDbContext context;
		private readonly IDataAccess<Group> dataAccess;
		public delegate void NodeMonitorHandler(object sender, NewNode node);
		public event NodeMonitorHandler NodeMonitor;

		string hubUrl = "";
		public SignalRService(OpcDbContext context, IDataAccess<Group> dataAccess, IMapper mapper)
		{
			//TODO: replace context with dataaccess repo
			this.context = context;
			this.dataAccess = dataAccess;
			//TODO: get rid of FoD and use based on a group
			if (Extensions.ReadSettings().SaveToDb && context.Database.CanConnect())
			{
				hubUrl = context.AppSettings.ToList().FirstOrDefault()?.SignalR?.HubUrl;
			}
			else
			{
				hubUrl = Extensions.ReadSettings().SignalR.HubUrl;
			}
			_hubConnection = new HubConnectionBuilder()
					.WithUrl(hubUrl)
					.Build();

			_hubConnection.Closed += (exception) =>
			{
				var isConnected = StartConnectionAsync().Result;
				return Task.CompletedTask;
			};
		}

		public async Task<bool> StartConnectionAsync()
		{
			try
			{
				if (_hubConnection is null || _hubConnection.State == HubConnectionState.Disconnected)
				{
					await _hubConnection.StartAsync();
				}

				Console.WriteLine(string.Format("# Connected to the {0} Hub", hubUrl));
				return true;
			}
			catch
			{
				LoggerManager.Logger.Error(string.Format("Cannot connect to SignalR: {0}", hubUrl));
				return false;
			}
		}

		#region actions
		public async Task JoinGroup(string groupName)
		{
			await _hubConnection.InvokeAsync("JoinGroup", groupName);
		}

		public async Task SendNodeAction(NodeData nodeData, string groupName = "All")
		{
			await _hubConnection.InvokeAsync("SendNodeAction", groupName, nodeData);
		}

		public async Task SendConfigNodesAction(string groupName, object nodesList)
		{
			var data = JsonConvert.SerializeObject(nodesList, new JsonSerializerSettings
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver()
			});

			await _hubConnection.InvokeAsync("SendConfigNodesAction", groupName, data);
		}

		public async Task SendGraphAction(string groupName, string treeData)
		{
			await _hubConnection.InvokeAsync("SendGraphAction", groupName, treeData);
		}

		public async Task SendSettingsAction(string groupName, object settings)
		{
			var data = JsonConvert.SerializeObject(settings, new JsonSerializerSettings
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver()
			});

			await _hubConnection.InvokeAsync("SendSettingsAction", groupName, data);
		}

		public async Task SendGroupsAction()
		{
			var data = JsonConvert.SerializeObject(await context.Groups.ToListAsync());

			await _hubConnection.InvokeAsync("SendGroupsAction", data);
		}

		#endregion

		#region listeners

		public void ListenGetGraph(string enpointName = "GetGraphWeb")
		{
			/// read graph from file and send
			_hubConnection.On<string, bool>(enpointName, async (groupName, isFullGraph) =>
			{
				var graphName = isFullGraph ? "graph-full.json" : "graph-new.json";
				var direcory = Path.Combine(Directory.GetCurrentDirectory(), graphName);
				var treeData = File.ReadAllText(direcory);
				await SendGraphAction(groupName, treeData);
			});
		}

		public void ListenNodeMonitor(string enpointName = "NodeMonitorWeb")
		{
			/// appsettings.json modify to monitor a node
			_hubConnection.On<object>(enpointName, async (nodeObj) =>
			{
				try
				{
					var node = JsonConvert.DeserializeObject<NewNode>(nodeObj.ToString());
					// trigger parse the tree again
					if (node.Action == MonitorAction.Monitor)
					{
						context.NodesConfig.Add(new BaseNode
						{
							NodeId = node.NodeId,
							MSecs = node.MSecs,
							Range = node.Range,
							Name = node.Name,
							Group = node.Group,
						});
					}
					if (node.Action == MonitorAction.Unmonitor)
					{
						var nodesToRemove = await context.NodesConfig.Where(c => c.Group == node.Group && c.NodeId == node.NodeId).ToListAsync();
						context.NodesConfig.RemoveRange(nodesToRemove);
					}
					await context.SaveChangesAsync();
					NodeMonitor?.Invoke(this, node);

					LoggerManager.Logger.Information(string.Format("Tree updated"));
					Console.WriteLine($"NodeId: {node.NodeId} group: {node.Group} action: {node.Action}");
					await Task.CompletedTask;
				}
				catch (Exception ex)
				{
					Console.WriteLine();
				}
			});
		}
		public void ListenGetNodes(string enpointName = "GetNodesWeb")
		{
			_hubConnection.On<string>(enpointName, async (groupName) =>
			{
				var nodesList = await context.NodesConfig.Where(c => c.Group == groupName).ToListAsync();
				//var settings = context.AppSettings.LastOrDefault();

				await SendConfigNodesAction(groupName, nodesList);
			});
		}

		public void ListenGetSettings(string enpointName = "GetSettingsWeb")
		{
			_hubConnection.On<string>(enpointName, async (groupName) =>
			{
				var group = await context.Groups.Include(c => c.AppSettings)
							.Include(g => g.AppSettings)
									.ThenInclude(a => a.AzureEventHub)
											.ThenInclude(ae => ae.EventHubSender)
							.Include(g => g.AppSettings)
									.ThenInclude(a => a.AzureEventHub)
											.ThenInclude(ae => ae.EventHubConsumer)
							.Include(g => g.AppSettings)
									.ThenInclude(c => c.SignalR)
							.FirstOrDefaultAsync(c => c.Name == groupName);
				//var settings = context.AppSettings.LastOrDefault();

				await SendSettingsAction(groupName, group);
			});
		}

		public void ListenSaveSettings(string enpointName = "SaveSettingsWeb")
		{
			_hubConnection.On<string, object>(enpointName, async (groupName, appSettings) =>
			{
				var group = context.Groups.FirstOrDefault(c => c.Name == groupName) ?? new Group();
				group.Name = groupName;
				group.AppSettings = JsonConvert.DeserializeObject<AppSettings>(appSettings.ToString());

				await dataAccess.UpdateSettingsAsync(group);

				await SendSettingsAction(groupName, group);
			});
		}

		public void ListenGetGroups(string enpointName = "GetGroupsWeb")
		{
			_hubConnection.On(enpointName, async () =>
			{
				await SendGroupsAction();
			});
		}

		public void ListenAddGroup(string enpointName = "AddGroupWeb")
		{
			/// read graph from file and send
			_hubConnection.On<string>(enpointName, async (groupName) =>
			{
				var group = await context.Groups.AddAsync(new Group() { Name = groupName });
				await context.SaveChangesAsync();
			});
		}

		public void ListenRemoveGroup(string enpointName = "RemoveGroupWeb")
		{
			/// read graph from file and send
			_hubConnection.On<string>(enpointName, async (groupName) =>
			{
				var groupToDelete = await context.Groups.FirstOrDefaultAsync(c => c.Name == groupName);
				context.Groups.Remove(groupToDelete);
				await context.SaveChangesAsync();
			});
		}
		#endregion

		public async Task StopConnection()
		{
			await _hubConnection.StopAsync();
		}
	}
}
