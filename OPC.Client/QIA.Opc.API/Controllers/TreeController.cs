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
		public ActionResult<TreeNode> TreeChildrens([FromQuery] string sessionNodeId, [FromBody] TreeNode treeItem)
		{
			if (treeItem.Children != null && treeItem.Children.Count > 0)
			{
				// should be loaded then already
				return Ok(treeItem.Children);
			}

			var result = treeService.BrowseChild(sessionNodeId, treeItem);

			return HandleResponse(result);
		}

		[HttpGet("full")]
		public async Task<IActionResult> DownloadFullTree([FromQuery] string sessionNodeId)
		{
			var treeResponse = await treeService.GetFullGraphAsync(sessionNodeId);
			var byteArray = Encoding.UTF8.GetBytes(treeResponse.Value.Data);
			var stream = new MemoryStream(byteArray);

			return File(stream, "text/plain", treeResponse.Value.SourceName + ".txt");
		}
	}
}
