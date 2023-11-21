using Azure;
using Microsoft.AspNetCore.Mvc;
using Qia.Opc.Domain.Entities;
using QIA.Opc.Infrastructure.Services.OPCUA;
using System.Text;

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
		public async Task<IActionResult> BrowseTree()
		{
			var treeContainer = await treeService.GetFullGraphAsync();
			var byteArray = Encoding.UTF8.GetBytes(treeContainer.Data);
			var stream = new MemoryStream(byteArray);

			return File(stream, "text/plain", treeContainer.SourceName + ".txt");
		}
	}
}
