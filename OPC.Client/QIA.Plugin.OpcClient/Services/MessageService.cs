using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using QIA.Plugin.OpcClient.Core;
using QIA.Plugin.OpcClient.Entities;

namespace QIA.Plugin.OpcClient.Services
{
    public class MessageService
    {
        private readonly HttpClient _httpClient;
        public MessageService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            //_httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
        }

        public async Task SendMessage(NodeData message, string groupName = "All")
        {
            string json = JsonConvert.SerializeObject(message);

            StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            string query = "?group=" + groupName;
            var result = await _httpClient.PostAsync("https://localhost:7027/Data/SendMessage" + query, httpContent);
            if (!result.IsSuccessStatusCode)
            {
                var response = await result.Content.ReadAsStringAsync();
                LoggerManager.Logger.Error("Http message post error: {0}", response);
            }
        }
        public async Task SendGraph(string graphName, TreeNode<NodeData> graphTree, string groupName = "All")
        {
            string json = JsonConvert.SerializeObject(graphTree);
            await this.SendGraph(graphName, json, groupName);

        }

        public async Task SendGraph(string graphName, string graphTree, string groupName = "All")
        {
            string json = JsonConvert.SerializeObject(graphTree);

            StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            string query = "?groupName=" + groupName + "&graphName=" + graphName;
            var result = await _httpClient.PostAsync("https://localhost:7027/Data/SendGraph" + query, httpContent);
            if (!result.IsSuccessStatusCode)
            {
                var response = await result.Content.ReadAsStringAsync();
                LoggerManager.Logger.Error("Http message post error: {0}", response);
            }
        }
    }
}
