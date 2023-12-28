using Microsoft.AspNetCore.Mvc;
using Qia.Opc.Domain.Entities;
using QIA.Opc.Infrastructure.Services.OPCUA;
using System.Collections.Generic;

namespace QIA.Opc.API.Controllers
{
	public class NodeController : BaseController
	{
		private readonly NodeService nodeService;

		public NodeController(NodeService nodeService)
		{
			this.nodeService = nodeService;
		}

		[HttpGet("config/list")]
		public async Task<ActionResult<IEnumerable<MonitoredItemConfig>>> GetConfigNodes()
		{
			var nodesReponse = await nodeService.GetConfigNodesAsync();

			return HandleResponse(nodesReponse);
		}

		[HttpDelete("{nodeId}")]
		public async Task<IActionResult> DeleteConfigNode(string nodeId)
		{
			var response = await nodeService.DeleteConfigNodeAsync(nodeId);

			return HandleResponse(response);
		}


		[HttpPost("create")]
		public async Task<ActionResult<MonitoredItemConfig>> CreateConfigNode([FromBody] MonitoredItemConfig nodeReference)
		{
			var response = await nodeService.AddConfigNodeAsync(nodeReference);

			return HandleResponse(response);
		}
	}
}
