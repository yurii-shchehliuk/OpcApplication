using AutoMapper;
using MediatR;
using Opc.Ua;
using Qia.Opc.Domain.Common;
using Qia.Opc.Domain.Core;
using Qia.Opc.Domain.Entities;
using Qia.Opc.Infrastructure.Application;
using Qia.Opc.OPCUA.Connector.Entities;
using Qia.Opc.OPCUA.Connector.Managers;
using Qia.Opc.Persistence.Repository;
using System.Diagnostics;
using System.Reflection;

namespace QIA.Opc.Infrastructure.Services.OPCUA
{
	public class NodeService
	{
		private readonly SessionManager sessionManager;
		private readonly NodeManager nodeManager;
		private readonly IDataRepository<NodeReferenceEntity> nodeRepository;
		private readonly IDataRepository<NodeValue> nodeDataRepo;
		private readonly IDataRepository<SessionEntity> sessionRepo;
		private readonly IMediator mediator;

		public NodeService(SessionManager sessionManager,
					 NodeManager nodeManager,
					 IDataRepository<NodeReferenceEntity> nodeRepository,
					 IDataRepository<NodeValue> nodeDataRepo,
					 IDataRepository<SessionEntity> sessionRepo,
					 IMediator mediator)
		{
			this.sessionManager = sessionManager;
			this.nodeManager = nodeManager;
			this.nodeRepository = nodeRepository;
			this.nodeDataRepo = nodeDataRepo;
			this.sessionRepo = sessionRepo;
			this.mediator = mediator;
		}

		public async Task DeleteConfigNodeAsync(string nodeId)
		{
			var session = sessionManager.CurrentSession;
			var result = await nodeRepository.FindAsync(c => c.NodeId == nodeId && c.SessionEntity.Name == session.Name);
			if (result != null)
				await nodeRepository.DeleteAsync(result);
		}

		public async Task<IEnumerable<NodeReferenceEntity>> GetConfigNodesAsync()
		{
			return await nodeRepository.ListAllAsync(c => c.SessionEntity.Name == sessionManager.CurrentSession.Name);
		}

		public async Task<OperationResponse<NodeReferenceEntity>> AddConfigNodeAsync(NodeReferenceEntity nodeRef)
		{
			try
			{
				var currentNode = await nodeRepository.FindAsync(c => c.NodeId == nodeRef.NodeId);
				if (currentNode != null && currentNode.SubscriptionId != null)
				{
					await mediator.Publish(new EventMediatorCommand(new EventData
					{
						LogCategory = Qia.Opc.Domain.Entities.Enums.LogCategory.Error,
						Message = "Subscription is active",
						Title = "Node exception"
					}));
					return OperationResponse<NodeReferenceEntity>.CreateFailure(new string[] { "Subscription is active" }, System.Net.HttpStatusCode.Conflict);
				}

				//prepare node
				var node = nodeManager.FindNodeOnServer(nodeRef.NodeId);
				nodeRef.NodeClass = node.NodeClass;

				var session = sessionManager.CurrentSession;
				var sessionData = await sessionRepo.FindAsync(c => c.Name == session.Name);

				nodeRef.SessionEntityId = sessionData.Id;
				//update
				await nodeRepository.UpsertAsync(nodeRef, e => e.NodeId == nodeRef.NodeId);
				await mediator.Publish(new EventMediatorCommand(new EventData
				{
					LogCategory = Qia.Opc.Domain.Entities.Enums.LogCategory.Success,
					Message = "",
					Title = "Node updated"
				}));
				return OperationResponse<NodeReferenceEntity>.CreateSuccess(nodeRef);
			}
			catch (Exception ex)
			{
				await mediator.Publish(new EventMediatorCommand(new EventData
				{
					LogCategory = Qia.Opc.Domain.Entities.Enums.LogCategory.Warning,
					Message = ex.Message,
					Title = "Node exception"
				}));
				return OperationResponse<NodeReferenceEntity>.CreateFailure(new string[] { ex.Message, ex.InnerException.Message });

			}
		}

		public async Task UpsertConfigAsync(NodeReferenceEntity nodeRef)
		{
			var session = sessionManager.CurrentSession;
			if (nodeRef == null || session.State == Qia.Opc.Domain.Entities.Enums.SessionState.Disconnected)
				return;

			var sessionData = await sessionRepo.FindAsync(c => c.Name == session.Name);

			//update
			await nodeRepository.UpsertAsync(nodeRef, e => e.NodeId == nodeRef.NodeId && e.SessionEntityId == sessionData.Id);
		}

		public async Task<NodeReferenceEntity> FindNodeByNodeIdAsync(string nodeId)
		{
			var session = sessionManager.CurrentSession;
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
