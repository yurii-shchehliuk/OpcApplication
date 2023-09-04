using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using WebApplication1.Services;
using Microsoft.Extensions.Hosting;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNet.SignalR.Infrastructure;
using SignalR.Hub.Entities;

namespace QIA.Plugin.OpcClient.Services
{
    /// <summary>
    /// Sends a message through http request to hub clients
    /// </summary>
    public class SignalrService
    {
        private readonly IHubContext<ChatHub> hubContext;
        public SignalrService(IHubContext<ChatHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        /// <summary>
        /// recieves message from httpClient and sends to signalR to populate data
        /// </summary>
        /// <param name="nodeData"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        /// <remarks>because we cannot cause a circular dependency (signalR nodeMonitor -> subscription/Tree -> signalR)</remarks>
        public async Task SendMessage(string groupName, string nodeData)
        {
            if (string.IsNullOrEmpty(groupName) || groupName == "All")
            {
                await hubContext.Clients.All.SendAsync("MessageReceived", nodeData);
            }
            else
            {
                await hubContext.Clients.Group(groupName).SendAsync("MessageReceivedGroup", nodeData);
            }
        }

        public async Task SendGraph(string groupName, string graphName, string graphTree)
        {
            if (string.IsNullOrEmpty(groupName) || groupName == "All")
            {
                await hubContext.Clients.All.SendAsync("LoadGraph", groupName, graphName, graphTree);
            }
            else
            {
                await hubContext.Clients.Group(groupName).SendAsync("LoadGraphGroup", groupName, graphName, graphTree);
            }
        }
    }
}
