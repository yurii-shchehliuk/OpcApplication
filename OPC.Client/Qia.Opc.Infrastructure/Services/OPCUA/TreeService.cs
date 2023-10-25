using Qia.Opc.Domain.Entities;
using Qia.Opc.OPCUA.Connector.Managers;
using Qia.Opc.Persistence.Repository;

namespace Qia.Opc.Infrastrucutre.Services.OPCUA
{
	public class TreeService
	{
		private readonly TreeManager treeManager;
		private readonly IDataRepository<NodeReferenceEntity> nodeReferencesRepo;
		private readonly SubscriptionService subscriptionService;

		public TreeService(TreeManager treeManager, IDataRepository<NodeReferenceEntity> nodeReferences, SubscriptionService subscriptionService)
		{
			this.treeManager = treeManager;
			this.nodeReferencesRepo = nodeReferences;
			this.subscriptionService = subscriptionService;
			this.treeManager.NodeFound += TreeManager_NodeWasFound;
		}

		private async Task TreeManager_NodeWasFound(object sender, NodeReferenceEntity nodeRef)
		{
			await subscriptionService.SubscribeAsync(nodeRef);
			await nodeReferencesRepo.AddAsync(nodeRef);
		}

		public TreeNode GetFullGraph()
		{
			return treeManager.BrowseTree();
		}

		/// <summary>
		/// Adds empty node to values table.
		/// On found event or on getting from DB subscribe.
		/// </summary>
		/// <param name="configNodes"></param>
		/// <param name="groupName"></param>
		/// <returns></returns>
		public TreeNode FindNodesRecursively(HashSet<NodeReferenceEntity> configNodes)
		{
			return treeManager.FindNodesRecursively(configNodes);
		}

		public TreeNode BrowseChild(TreeNode node)
		{
			return treeManager.BrowseChildren(node);
		}
	}
}
