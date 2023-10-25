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
		public ActionResult<TreeNode> TreeChildrens([FromBody] TreeNode treeItem)
		{
			if (treeItem.Children != null && treeItem.Children.Count > 0)
			{
				// should be loaded then already
				return Ok(treeItem.Children);
			}

			var result = treeService.BrowseChild(treeItem);
			return Ok(result.Children);
		}

		[HttpGet("full")]
		public IActionResult BrowseTree()
		{
			var graph = treeService.GetFullGraph();
			return Ok(graph);
		}

		[HttpPost("active")]
		public ActionResult<TreeNode> FindNodesRecursively([FromBody] HashSet<NodeReferenceEntity> nodesToFind)
		{
			var result = treeService.FindNodesRecursively(nodesToFind);
			return Ok(result);
		}
	}
}
