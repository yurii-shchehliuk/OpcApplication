using Newtonsoft.Json;
using Qia.Opc.Domain.Entities;
using Qia.Opc.OPCUA.Connector.Managers;
using Qia.Opc.Persistence.Repository;

namespace QIA.Opc.Infrastructure.Services.OPCUA
{
	public class TreeService
	{
		private readonly TreeManager treeManager;
		private readonly IDataRepository<TreeContainer> treeRepo;
		private readonly SessionManager sessionManager;

		public TreeService(TreeManager treeManager,
					 IDataRepository<TreeContainer> treeRepo,
					 SessionManager sessionManager)
		{
			this.treeManager = treeManager;
			this.treeRepo = treeRepo;
			this.sessionManager = sessionManager;
		}

		public async Task<TreeContainer> GetFullGraphAsync()
		{
			var treeData = await treeManager.BrowseTreeAsync();
			var textContent = JsonConvert.SerializeObject(treeData);
			TreeContainer treeContainer = new TreeContainer()
			{
				Data = textContent,
				SourceName = sessionManager.CurrentSession.Name,
				StoreTime = DateTime.UtcNow
			};
			await treeRepo.AddAsync(treeContainer);

			return treeContainer;

		}

		public TreeNode BrowseChild(TreeNode node)
		{
			return treeManager.BrowseChildren(node);
		}
	}
}
