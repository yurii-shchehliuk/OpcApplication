using MediatR;
using Qia.Opc.Domain.Core;
using Qia.Opc.Domain.Entities;
using Qia.Opc.OPCUA.Connector.Managers;
using QIA.Opc.Domain.Repository;

namespace QIA.Opc.Infrastructure.Services.OPCUA
{
	public class NodeService
	{
		private readonly SessionManager sessionManager;
		private readonly NodeManager nodeManager;
		private readonly IGenericRepository<MonitoredItemConfig> nodeRepository;
		private readonly IGenericRepository<MonitoringItemValue> nodeDataRepo;
		private readonly IGenericRepository<SessionEntity> sessionRepo;

		public NodeService(SessionManager sessionManager,
					 NodeManager nodeManager,
					 IGenericRepository<MonitoredItemConfig> nodeRepository,
					 IGenericRepository<MonitoringItemValue> nodeDataRepo,
					 IGenericRepository<SessionEntity> sessionRepo)
		{
			this.sessionManager = sessionManager;
			this.nodeManager = nodeManager;
			this.nodeRepository = nodeRepository;
			this.nodeDataRepo = nodeDataRepo;
			this.sessionRepo = sessionRepo;
		}

		public async Task<ApiResponse<MonitoredItemConfig>> DeleteConfigNodeAsync(string nodeId)
		{
			var session = sessionManager.CurrentSession;
			//var result = await nodeRepository.FindAsync(c => c.StartNodeId == nodeId && c.SessionEntity.Name == session.Name);
			//if (result == null) return ApiResponse<MonitoredItemConfig>.Failure(HttpStatusCode.NotFound);

			//await nodeRepository.DeleteAsync(result);
			return ApiResponse<MonitoredItemConfig>.Success();
		}

		public async Task<ApiResponse<IEnumerable<MonitoredItemConfig>>> GetConfigNodesAsync()
		{
			//var result = await nodeRepository.ListAllAsync(c => c.SessionEntity.Name == sessionManager.CurrentSession.Name);
			return ApiResponse<IEnumerable<MonitoredItemConfig>>.Success();
		}

		public async Task<ApiResponse<MonitoredItemConfig>> AddConfigNodeAsync(MonitoredItemConfig nodeRef)
		{
			try
			{
				var currentNode = await nodeRepository.FindAsync(c => c.StartNodeId == nodeRef.StartNodeId);
				if (currentNode != null && currentNode.SubscriptionId != null)
				{
					return ApiResponse<MonitoredItemConfig>.Failure(HttpStatusCode.Conflict, "Subscription is active");
				}

				//prepare node
				var node = nodeManager.FindNodeOnServer(nodeRef.StartNodeId);
				nodeRef.NodeClass = node.NodeClass;

				var session = sessionManager.CurrentSession;
				var sessionData = await sessionRepo.FindAsync(c => c.Name == session.Name);

				//nodeRef.SessionEntityId = sessionData.Id;
				//update
				await nodeRepository.UpsertAsync(nodeRef, e => e.StartNodeId == nodeRef.StartNodeId);

				return ApiResponse<MonitoredItemConfig>.Success(nodeRef, HttpStatusCode.Created);
			}
			catch (Exception ex)
			{
				return ApiResponse<MonitoredItemConfig>.Failure(HttpStatusCode.BadRequest, ex.Message);
			}
		}

		public async Task<ApiResponse<MonitoredItemConfig>> UpsertConfigAsync(MonitoredItemConfig nodeRef)
		{
			var session = sessionManager.CurrentSession;
			if (nodeRef == null || session.State == Qia.Opc.Domain.Entities.Enums.SessionState.Disconnected)
				return ApiResponse<MonitoredItemConfig>.Failure(HttpStatusCode.ServiceUnavailable);

			var sessionData = await sessionRepo.FindAsync(c => c.Name == session.Name);

			//update
			//await nodeRepository.UpsertAsync(nodeRef, e => e.StartNodeId == nodeRef.StartNodeId && e.SessionEntityId == sessionData.Id);

			return ApiResponse<MonitoredItemConfig>.Success(nodeRef);
		}

		public async Task<ApiResponse<MonitoredItemConfig>> FindNodeByNodeIdAsync(string nodeId)
		{
			var session = sessionManager.CurrentSession;
			var sessionData = await sessionRepo.FindAsync(c => c.Name == session.Name);
			if (sessionData == null)
			{
				return ApiResponse<MonitoredItemConfig>.Failure(HttpStatusCode.NotFound);
			}
			//var result = await nodeRepository.FindAsync(c => c.StartNodeId == nodeId && c.SessionEntityId == sessionData.Id);
			return ApiResponse<MonitoredItemConfig>.Success();
		}

		public async Task AddNodeValueToDbAsync(MonitoringItemValue nodeData)
		{
			nodeData.Id = 0;
			await nodeDataRepo.AddAsync(nodeData);
		}
	}
}
