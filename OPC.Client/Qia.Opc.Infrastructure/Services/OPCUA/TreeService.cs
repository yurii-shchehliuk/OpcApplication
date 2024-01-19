using Newtonsoft.Json;
using Qia.Opc.Domain.Core;
using Qia.Opc.Domain.Entities;
using Qia.Opc.OPCUA.Connector.Managers;
using QIA.Opc.Domain.Repository;

namespace QIA.Opc.Infrastructure.Services.OPCUA
{
	public class TreeService
	{
		private readonly TreeManager treeManager;
		private readonly IGenericRepository<TreeContainer> treeRepo;
		private readonly SessionManager sessionManager;

		public TreeService(TreeManager treeManager,
					 IGenericRepository<TreeContainer> treeRepo,
					 SessionManager sessionManager)
		{
			this.treeManager = treeManager;
			this.treeRepo = treeRepo;
			this.sessionManager = sessionManager;
		}

		public async Task<ApiResponse<TreeContainer>> GetFullGraphAsync(string sessionGuid)
		{
			var treeData = await treeManager.BrowseTreeAsync(sessionGuid);
			var textContent = JsonConvert.SerializeObject(treeData);
			sessionManager.TryGetSession(sessionGuid, out var session);
			TreeContainer treeContainer = new TreeContainer()
			{
				Data = textContent,
				SourceName = session.Name,
				CreatedAt = DateTime.UtcNow
			};
			await treeRepo.AddAsync(treeContainer);

			return ApiResponse<TreeContainer>.Success(treeContainer);

		}

		public ApiResponse<TreeNode> BrowseChild(string sessionGuid, TreeNode node)
		{
			var result = treeManager.BrowseChildren(sessionGuid, node);
			return ApiResponse<TreeNode>.Success(result);
		}
	}
}
