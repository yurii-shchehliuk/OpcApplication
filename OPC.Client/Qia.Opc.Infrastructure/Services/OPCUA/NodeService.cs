using AutoMapper;
using Opc.Ua;
using Qia.Opc.Domain.Entities;
using Qia.Opc.OPCUA.Connector.Managers;
using Qia.Opc.Persistence.Repository;

namespace Qia.Opc.Infrastrucutre.Services.OPCUA
{
	public class NodeService
	{
		private readonly NodeManager nodeManager;
		private readonly IDataRepository<NodeReferenceEntity> nodeRepository;
		private readonly IDataRepository<NodeValue> nodeDataRepo;

		public NodeService(NodeManager nodeManager, IDataRepository<NodeReferenceEntity> nodeRepository, IDataRepository<NodeValue> nodeDataRepo)
		{
			this.nodeManager = nodeManager;
			this.nodeRepository = nodeRepository;
			this.nodeDataRepo = nodeDataRepo;
		}

		public async Task DeleteConfigNode(string nodeId)
		{
			var result = await nodeRepository.FindAsync(c => c.NodeId == nodeId);
			if (result != null)
				await nodeRepository.DeleteAsync(result);
		}

		public async Task<IEnumerable<NodeReferenceEntity>> GetConfigNodes()
		{
			return await nodeRepository.ListAllAsync();
		}

		public async Task UpsertConfig(NodeReferenceEntity nodeRef)
		{
			var node = nodeManager.FindNode(nodeRef.NodeId);
			nodeRef.NodeClass = node.NodeClass;
			await nodeRepository.UpsertAsync(nodeRef, e => e.NodeId == nodeRef.NodeId);
		}

		internal async Task<NodeReferenceEntity> FindNodeByNodeIdAsync(string nodeId)
		{
			return await nodeRepository.FindAsync(c => c.NodeId == nodeId);
		}

		internal async Task AddNodeValueAsync(NodeValue nodeData)
		{
			await nodeDataRepo.AddAsync(nodeData);
		}

		public Node FindNodeOnServer(string nodeId)
		{
			return nodeManager.FindNode(nodeId);
		}

		public DataValue ReadNodeValueOnServer(string nodeId)
		{
			return nodeManager.ReadValue(nodeId);
		}
	}
}
