using Microsoft.AspNetCore.Mvc;
using Qia.Opc.Domain.Entities;
using Qia.Opc.Infrastrucutre.Services.OPCUA;

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
		public async Task<ActionResult<IEnumerable<NodeReferenceEntity>>> GetConfigNodes()
		{
			var result = await nodeService.GetConfigNodes();
			return Ok(result);
		}

		[HttpDelete("{nodeId}")]
		public async Task<IActionResult> DeleteConfigNode(string nodeId)
		{
			await nodeService.DeleteConfigNode(nodeId);
			return Ok();
		}
	}
}
