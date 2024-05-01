namespace QIA.Opc.API.Controllers;

using System.Text;
using Microsoft.AspNetCore.Mvc;
using Qia.Opc.Domain.Entities;
using QIA.Opc.Infrastructure.Services.OPCUA;

public class TreeController : BaseController
{
    private readonly TreeService _treeService;

    public TreeController(TreeService treeService)
    {
        _treeService = treeService;
    }

    [HttpPatch("childrens")]
    public ActionResult<TreeNode> TreeChildrens([FromQuery] string sessionNodeId, [FromBody] TreeNode treeItem)
    {
        if (treeItem.Children != null && treeItem.Children.Count > 0)
        {
            // should be loaded then already
            return Ok(treeItem.Children);
        }

        Infrastructure.Application.ApiResponse<TreeNode> result = _treeService.BrowseChild(sessionNodeId, treeItem);

        return HandleResponse(result);
    }

    [HttpGet("full")]
    public async Task<IActionResult> DownloadFullTree([FromQuery] string sessionNodeId)
    {
        Infrastructure.Application.ApiResponse<TreeContainer> treeResponse = await _treeService.GetFullGraphAsync(sessionNodeId);
        var byteArray = Encoding.UTF8.GetBytes(treeResponse.Value.Data);
        MemoryStream stream = new(byteArray);

        return File(stream, "text/plain", treeResponse.Value.SourceName + ".txt");
    }
}
