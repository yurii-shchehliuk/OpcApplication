using Microsoft.AspNetCore.Mvc;
using Qia.Opc.Domain.Entities;
using Qia.Opc.Infrastrucutre.Services.OPCUA;

namespace QIA.Opc.API.Controllers
{

	public class TreeController : BaseController
	{
		private readonly TreeService treeService;

		public TreeController(TreeService treeService)
		{
			this.treeService = treeService;
		}

		[HttpPatch("childrens")]
		public async Task<ActionResult<TreeNode>> TreeChildrens([FromBody] TreeNode treeItem)
		{
			if (treeItem.Children.Count > 0)
			{
				// should be loaded then already
				return Ok();
			}

			var result = treeService.BrowseChild(treeItem);
			return Ok(result);
		}

		[HttpGet("full")]
		public IActionResult BrowseTree()
		{
			var graph = treeService.GetFullGraph();
			return Ok(graph);
		}

		[HttpPost("active")]
		public async Task<ActionResult<TreeNode>> FindNodesRecursively([FromBody] HashSet<NodeReferenceEntity> nodesToFind)
		{
			var result =  treeService.FindNodesRecursively(nodesToFind);
			return Ok();
		}

	}
}
