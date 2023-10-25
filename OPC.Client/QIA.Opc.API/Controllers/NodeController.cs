using Microsoft.AspNetCore.Mvc;
using Opc.Ua;
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
			var result = await nodeService.GetConfigNodesAsync();
			return Ok(result);
		}

		[HttpDelete("{nodeId}")]
		public async Task<IActionResult> DeleteConfigNode(string nodeId)
		{
			await nodeService.DeleteConfigNodeAsync(nodeId);
			return Ok();
		}


		[HttpPost("create")]
		public async Task<ActionResult<NodeReferenceEntity>> CreateConfigNode([FromBody] NodeReferenceEntity nodeReference)
		{
			await nodeService.AddConfigNodeAsync(nodeReference);
			return Ok();
		}
	}
}
