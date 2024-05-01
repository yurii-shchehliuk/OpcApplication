namespace QIA.Opc.Infrastructure.Services.OPCUA;

using Newtonsoft.Json;
using Qia.Opc.Domain.Entities;
using QIA.Opc.Infrastructure.Application;
using QIA.Opc.Infrastructure.Managers;

public class TreeService
{
    private readonly TreeManager _treeManager;
    private readonly SessionManager _sessionManager;

    public TreeService(TreeManager treeManager,
                 SessionManager sessionManager)
    {
        _treeManager = treeManager;
        _sessionManager = sessionManager;
    }

    public async Task<ApiResponse<TreeContainer>> GetFullGraphAsync(string sessionGuid)
    {
        TreeNode treeData = await _treeManager.BrowseTreeAsync(sessionGuid);
        var textContent = JsonConvert.SerializeObject(treeData);
        _sessionManager.TryGetSession(sessionGuid, out Entities.OPCUASession session);
        TreeContainer treeContainer = new()
        {
            Data = textContent,
            SourceName = session.Name,
            CreatedAt = DateTime.UtcNow
        };

        return ApiResponse<TreeContainer>.Success(treeContainer);
    }

    public ApiResponse<TreeNode> BrowseChild(string sessionGuid, TreeNode node)
    {
        TreeNode result = _treeManager.BrowseChildren(sessionGuid, node);
        return ApiResponse<TreeNode>.Success(result);
    }
}
