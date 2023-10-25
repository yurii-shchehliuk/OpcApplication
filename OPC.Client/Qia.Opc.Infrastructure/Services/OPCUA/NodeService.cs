using AutoMapper;
using Opc.Ua;
using Qia.Opc.Domain.Entities;
using Qia.Opc.OPCUA.Connector.Entities;
using Qia.Opc.OPCUA.Connector.Managers;
using Qia.Opc.Persistence.Repository;

namespace Qia.Opc.Infrastrucutre.Services.OPCUA
{
	public class NodeService
	{
		private readonly OPCUASession opcSession;
		private readonly NodeManager nodeManager;
		private readonly IDataRepository<NodeReferenceEntity> nodeRepository;
		private readonly IDataRepository<NodeValue> nodeDataRepo;
		private readonly IDataRepository<SessionEntity> sessionRepo;
		private readonly SessionService sessionService;

		public NodeService(SessionManager sessionManager,
		NodeManager nodeManager, IDataRepository<NodeReferenceEntity> nodeRepository, IDataRepository<NodeValue> nodeDataRepo, IDataRepository<SessionEntity> sessionRepo, SessionService sessionService)
		{
			this.opcSession = sessionManager.CurrentSession;
			this.nodeManager = nodeManager;
			this.nodeRepository = nodeRepository;
			this.nodeDataRepo = nodeDataRepo;
			this.sessionRepo = sessionRepo;
			this.sessionService = sessionService;
		}

		public async Task DeleteConfigNodeAsync(string nodeId)
		{
			var session = sessionService.GetCurrentSession();
			var result = await nodeRepository.FindAsync(c => c.NodeId == nodeId && c.SessionEntity.Name == session.Name);
			if (result != null)
				await nodeRepository.DeleteAsync(result);
		}

		public async Task<IEnumerable<NodeReferenceEntity>> GetConfigNodesAsync()
		{
			return await nodeRepository.ListAllAsync(c => c.SessionEntity.Name == opcSession.Name);
		}

		public async Task AddConfigNodeAsync(NodeReferenceEntity nodeRef)
		{
			//prepare node
			var node = nodeManager.FindNodeOnServer(nodeRef.NodeId);
			nodeRef.NodeClass = node.NodeClass;

			var session = sessionService.GetCurrentSession();
			var sessionData = await sessionRepo.FindAsync(c => c.Name == session.Name);

			nodeRef.SessionEntityId = sessionData.Id;
			//update
			await nodeRepository.UpsertAsync(nodeRef, e => e.NodeId == nodeRef.NodeId);
		}

		public async Task UpsertConfigAsync(NodeReferenceEntity nodeRef)
		{
			var session = sessionService.GetCurrentSession();
			var sessionData = await sessionRepo.FindAsync(c => c.Name == session.Name);

			//update
			await nodeRepository.UpsertAsync(nodeRef, e => e.NodeId == nodeRef.NodeId && e.SessionEntityId == sessionData.Id);
		}

		public async Task<NodeReferenceEntity> FindNodeByNodeIdAsync(string nodeId)
		{
			var session = sessionService.GetCurrentSession();
			var sessionData = await sessionRepo.FindAsync(c => c.Name == session.Name);

			return await nodeRepository.FindAsync(c => c.NodeId == nodeId && c.SessionEntityId == sessionData.Id);
		}

		public async Task AddNodeValueAsync(NodeValue nodeData)
		{
			nodeData.Id = 0;
			await nodeDataRepo.AddAsync(nodeData);
		}

		public Node FindNodeOnServer(string nodeId)
		{
			return nodeManager.FindNodeOnServer(nodeId);
		}

		public DataValue ReadNodeValueOnServer(string nodeId)
		{
			return nodeManager.ReadValue(nodeId);
		}
	}
}
