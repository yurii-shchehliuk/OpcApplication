using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using QIA.Plugin.OpcClient.Core;
using QIA.Plugin.OpcClient.Core.Settings;
using QIA.Plugin.OpcClient.DTOs;
using QIA.Plugin.OpcClient.Entities;
using QIA.Plugin.OpcClient.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace QIA.Plugin.OpcClient.Services
{
    public class SignalRService
    {
        private readonly IConfigurationRoot _cfgRoot;
        private readonly ITreeManager _treeManager;
        private readonly MessageService messageService;
        private readonly string hubUrl;
        private readonly HubConnection _hubConnection;

        public SignalRService(IConfigurationRoot cfgRoot, ITreeManager treeMgr, MessageService messageService)
        {
            this._cfgRoot = cfgRoot;
            this._treeManager = treeMgr;
            this.messageService = messageService;
            hubUrl = _cfgRoot["Workers:OPC_UA.QGH01:Config:SignalR:HubUrl"];

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .Build();
        }

        public async Task StartConnectionAsync()
        {
            try
            {
                if (_hubConnection is null || _hubConnection.State == HubConnectionState.Disconnected)
                {
                    await _hubConnection.StartAsync();
                }

                Console.WriteLine(string.Format("# Connected to the {0} Hub", hubUrl));
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                LoggerManager.Logger.Error(ex, string.Format("Cannot connect to {0}: {1}", hubUrl, ex.Message));
            }
        }
      
        public async Task SendMessage(NodeData nodeData, string groupName = "All")
        {
            await _hubConnection.InvokeAsync("SendMessage", groupName, nodeData);
        }

        public async Task JoinGroup(string groupName)
        {
            await _hubConnection.InvokeAsync("JoinGroup", groupName);
        }

        public async Task SendGraph(string graphName, TreeNode<NodeData> treeData, string groupName = "All")
        {
            await _hubConnection.InvokeAsync("SendGraphAction", groupName, graphName, JsonSerializer.Serialize(treeData));
        }
        public async Task SendGraph(string graphName, string treeData, string groupName = "All")
        {
            await _hubConnection.InvokeAsync("SendGraphAction", groupName, graphName, treeData);
        }


        public void ListenGraphSend(string enpointName = "GetGraph")
        {
            /// read graph from file and send
            _hubConnection.On<string, string>(enpointName, async (groupName, graphName) =>
            {
                var direcory = Path.Combine(Directory.GetCurrentDirectory(), graphName);
                var graphData = File.ReadAllText(direcory);
                await messageService.SendGraph("graphName", graphData);
                //await SendGraph(graphName, "here graph data comes");
            });

            //test does accepts message
            _hubConnection.On<string, string,string>("LoadGraph", async (groupName, graphName, graphTree) =>
            {
                Console.WriteLine(graphTree);
                await Task.CompletedTask;
            });

        }


        public void ListenMessages(string enpointName = "NodeMonitor")
        {
            /// appsettings.json modify to monitor a node
            _hubConnection.On<NewNode>(enpointName, async (node) =>
            {
                var _appSettings = new AppSettings(_cfgRoot);

                // trigger parse the tree again
                switch (node.Action)
                {
                    case MonitorAction.Monitor:
                        //TODO: the probelm is that parsing will be executed every call
                        _appSettings.NodeManager.Add(new NodeConfig
                        {
                            NodeId = node.NodeId,
                            Msecs = node.MSecs,
                            Range = node.Range,
                            Name = node.Name,
                            NodeType = NodeType.Subscription
                        });

                        var result = _treeManager.FindNodesRecursively(_appSettings);

                        break;
                    case MonitorAction.Unmonitor:
                        _appSettings.NodeManager.RemoveWhere(c => c.NodeId == node.NodeId);
                        break;
                    default:
                        break;
                }


                // overwrite appsettings.sjon
                var content = JsonSerializer.Serialize(_appSettings);
                content = "{\"Workers\": { \"OPC_UA.QGH01\":{\"Config\":" + content + "}}}";
                File.WriteAllText(Program.path, content);
                _cfgRoot.Reload();

                LoggerManager.Logger.Information(string.Format("Tree updated"));
                Console.WriteLine($"NodeId: {node.NodeId} group: {node.Group} action: {node.Action}");
                await Task.CompletedTask;
            });

        }

        public async Task StopConnection()
        {
            await _hubConnection.StopAsync();
        }
    }
}
