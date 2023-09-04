using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using QIA.Plugin.OpcClient.Services;
using SignalR.Hub.Entities;
using System.Text.Json;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataController : ControllerBase
    {
        private readonly SignalrService signalrService;

        public DataController(SignalrService signalrService)
        {
            this.signalrService = signalrService;
        }

        [HttpPost("SendMessage")]
        public async Task SendMessage([FromBody] NodeData nodeData, [FromQuery] string group)
        {
            await signalrService.SendMessage(group, JsonConvert.SerializeObject(nodeData));
        }

        [HttpPost("SendGraph")]
        public async Task SendGraph([FromBody] string graphTree, [FromQuery] string groupName, string graphName)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<TreeNode<NodeData>>(graphTree);
                await signalrService.SendGraph(groupName, graphName, graphTree);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}