using AutoMapper;
using Microsoft.AspNetCore.Http;
using Opc.Ua;
using Opc.Ua.Client;
using Qia.Opc.Domain.Core;
using Qia.Opc.Domain.DTO;
using Qia.Opc.Domain.Entities;
using Qia.Opc.Infrastrucutre.Services.Communication;
using Qia.Opc.OPCUA.Connector.Managers;
using Qia.Opc.Persistence.Repository;
using System.Reflection;
using ISession = Opc.Ua.Client.ISession;

namespace Qia.Opc.Infrastrucutre.Services.OPCUA
{
	public class SubscriptionService
	{
		private readonly SessionManager sessionManager;
		private readonly SubscriptionManager subscriptionManager;
		private readonly SignalRService signalRService;
		private readonly IMapper mapper;
		private readonly NodeService nodeService;
		private readonly SessionService sessionService;

		//
		private readonly ISession session;
		private List<NodeValue> tempNodes = new();

		public SubscriptionService(SessionManager sessionManager, SubscriptionManager subscriptionManager, SignalRService signalRService, NodeService nodeService, SessionService sessionService, IMapper mapper)
		{
			session = sessionManager.CurrentSession.Session;
			this.sessionManager = sessionManager;
			this.subscriptionManager = subscriptionManager;
			this.signalRService = signalRService;
			this.mapper = mapper;
			this.nodeService = nodeService;
			this.sessionService = sessionService;
			subscriptionManager.NodeMonitorUpdate += SubscriptionManager_MonitoredItemUpdate;
			subscriptionManager.SessionEvent += SubscriptionManager_SessionEvent;
		}

		public async Task<NodeReferenceEntity> SubscribeAsync(NodeReferenceEntity nodeRef)
		{
			var newNodeRef = subscriptionManager.Subscribe(nodeRef);
			await nodeService.UpsertConfigAsync(newNodeRef);
			return newNodeRef;
		}

		public async Task DeleteSubscriptionAsync(string subscriptionId, string nodeId)
		{
			var subscription = uint.Parse(subscriptionId);
			subscriptionManager.DeleteSubscription(subscription);
			var nodeRef = await nodeService.FindNodeByNodeIdAsync(nodeId);
			nodeRef.SubscriptionId = null;
			await nodeService.UpsertConfigAsync(nodeRef);
		}

		public IEnumerable<SubscriptionDTO> GetActiveSubscriptions()
		{
			var subscriptions = subscriptionManager.GetActiveSubscriptions();
			var subscriptionsDTO = mapper.Map<IEnumerable<SubscriptionDTO>>(subscriptions);
			return subscriptionsDTO;
		}

		public IEnumerable<MonitoredItem> GetAllMonitoredItems()
		{
			return subscriptionManager.GetAllMonitoredItems();
		}

		private async Task AddByMillisecondsAsync(NodeValue nodeData)
		{
			if (nodeData.MSecs == null || nodeData.MSecs <= 0)
				return;

			var lastStored = tempNodes.Where(c => c.NodeId == nodeData.NodeId).FirstOrDefault();

			if (lastStored == null || ((nodeData.StoreTime - lastStored.StoreTime.Value).Value.TotalSeconds <= nodeData.MSecs))
			{
				tempNodes.Add(nodeData);
			}
			else
			{
				await AddNewNodeAsync(nodeData, "msecs");
				tempNodes.RemoveAll(c => c.NodeId == nodeData.NodeId);
			}
		}

		private async Task AddByRangeAsync(NodeValue nodeData)
		{
			if (nodeData.Range == null || nodeData.Range <= 0)
				return;

			int tempDbCount = tempNodes.Where(c => c.NodeId == nodeData.NodeId).Count();

			// update node per range
			if (tempDbCount < nodeData.Range)
			{
				tempNodes.Add(nodeData);
			}
			else
			{
				await AddNewNodeAsync(nodeData, "range");
				tempNodes.RemoveAll(c => c.NodeId == nodeData.NodeId);
			}
		}

		private async Task AddNewNodeAsync(NodeValue nodeData, string monitoringCategory)
		{
			LoggerManager.Logger.Information($"[monitoring {monitoringCategory}] {nodeData.DisplayName} {nodeData.Value}");

			await nodeService.AddNodeValueAsync(nodeData);
			await signalRService.SendNodeAsync(nodeData, nodeData.SessionName);
			//await _azureMS.SendNodeAsync(entity);
		}

		private async void SubscriptionManager_MonitoredItemUpdate(object sender, NodeValue node)
		{
			NodeReferenceEntity nodeConfig = await nodeService.FindNodeByNodeIdAsync(node.NodeId);
			var sessionName = await sessionService.FindSessionAsync((int)nodeConfig.SessionEntityId);

			node.SessionName = sessionName.Name;
			node.DisplayName = nodeConfig.DisplayName;
			node.MSecs = nodeConfig.MSecs;
			node.Range = nodeConfig.Range;

			node.StoreTime = DateTime.Now;
			await AddByMillisecondsAsync(node);
			await AddByRangeAsync(node);
		}

		/// <summary>
		/// avoid stackoverflow
		/// </summary>
		private bool isHandlingEvent = false;
		private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
		private async void SubscriptionManager_SessionEvent(object senderSession, NotificationEventArgs e)
		{
			if (isHandlingEvent)
			{
				isHandlingEvent = false;
				return;
			}
			isHandlingEvent = true;
			await _semaphore.WaitAsync();

			try
			{
				foreach (var subscriptionItems in session.Subscriptions)
				{
					foreach (var monitoringItem in subscriptionItems.MonitoredItems)
					{
						string currentNodeId = monitoringItem.ResolvedNodeId.ToString();
						var nodeRefConfig = await nodeService.FindNodeByNodeIdAsync(currentNodeId);

						if (nodeRefConfig != null)
						{
							var nodeValueNew = nodeService.ReadNodeValueOnServer(monitoringItem.ResolvedNodeId.ToString());
							var sessionName = await sessionService.FindSessionAsync((int)nodeRefConfig.SessionEntityId);
							NodeValue nodeData = new()
							{
								SessionName = sessionName.Name,
								DisplayName = nodeRefConfig.DisplayName,
								StoreTime = DateTime.Now,
								Value = nodeValueNew?.Value.ToString(),
								NodeId = currentNodeId,
								MSecs = nodeRefConfig.MSecs,
								Range = nodeRefConfig.Range,
							};

							if (nodeRefConfig.SubscriptionId == null)
							{
								nodeRefConfig.SubscriptionId = subscriptionItems.Id.ToString();
								await nodeService.UpsertConfigAsync(nodeRefConfig);
							}

							await AddByMillisecondsAsync(nodeData);
							await AddByRangeAsync(nodeData);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LoggerManager.Logger.Error(ex.Message, ex);
			}
			finally
			{
				Thread.Sleep(200);
				_semaphore.Release();
				isHandlingEvent = false;
			}
		}
	}
}
