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
		//
		private readonly ISession session;
		private List<NodeValue> tempNodes = new();

		public SubscriptionService(SessionManager sessionManager, SubscriptionManager subscriptionManager, SignalRService signalRService, NodeService nodeService, IMapper mapper)
		{
			session = sessionManager.CurrentSession.Session;
			this.sessionManager = sessionManager;
			this.subscriptionManager = subscriptionManager;
			this.signalRService = signalRService;
			this.mapper = mapper;
			this.nodeService = nodeService;
			subscriptionManager.NodeMonitorUpdate += SubscriptionManager_MonitoredItemUpdate;
			subscriptionManager.SessionEvent += SubscriptionManager_SessionEvent;
		}

		public NodeReferenceEntity Subscribe(NodeReferenceEntity nodeRef)
		{
			return subscriptionManager.Subscribe(nodeRef);
		}

		public void DeleteSubscription(string subscriptionId)
		{
			var subscription = uint.Parse(subscriptionId);
			subscriptionManager.DeleteSubscription(subscription);
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

		private void AddByMilliseconds(NodeValue nodeData)
		{
			if (nodeData.MSecs <= 0 || nodeData.StoreTime == null)
				return;

			var lastStored = tempNodes.Where(c => c.NodeId == nodeData.NodeId).OrderByDescending(c => c.StoreTime).FirstOrDefault();

			if (lastStored == null)
			{
				AddNewNode(nodeData, "msecs");
			}
			else if ((lastStored.StoreTime.Value - nodeData.StoreTime).Value.TotalMilliseconds >= nodeData.MSecs)
			{
				AddNewNode(nodeData, "msecs");

			}
		}

		private void AddByRange(NodeValue nodeData)
		{
			if (nodeData.Range == 0)
				return;

			int tempDbCount = tempNodes.Where(c => c.NodeId == nodeData.NodeId).Count();

			// update node per range
			if (tempDbCount < nodeData.Range)
			{
				tempNodes.Add(nodeData);
			}
			else
			{
				AddNewNode(nodeData, "range");
				tempNodes.RemoveAll(c => c.NodeId == nodeData.NodeId);
			}
		}

		private async Task AddNewNode(NodeValue nodeData, string monitoringCategory)
		{
			LoggerManager.Logger.Information($"[monitoring {monitoringCategory}] {nodeData.DisplayName} {nodeData.Value} {DateTimeOffset.UtcNow}");

			await nodeService.AddNodeValueAsync(nodeData);
			await signalRService.SendNodeAction(nodeData, nodeData.SessionName);
			//await _azureMS.SendNodeAsync(entity);
		}

		private void SubscriptionManager_MonitoredItemUpdate(object sender, NodeValue node)
		{
			NodeReferenceEntity nodeConfig = nodeService.FindNodeByNodeIdAsync(node.NodeId).Result;

			node.DisplayName = nodeConfig.DisplayName;
			node.MSecs = nodeConfig.MSecs;
			node.Range = nodeConfig.Range;

			node.SessionName = sessionManager.CurrentSession.Name;
			node.StoreTime = DateTime.Now;
			AddByMilliseconds(node);
			AddByRange(node);
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

							NodeValue nodeData = new()
							{
								SessionName = sessionManager.CurrentSession.Name,
								DisplayName = nodeRefConfig.DisplayName,
								StoreTime = DateTime.Now,
								Value = nodeValueNew.Value.ToString(),
								NodeId = currentNodeId,
								MSecs = nodeRefConfig.MSecs,
								Range = nodeRefConfig.Range,
							};

							//if (nodeRefConfig == null && nodeValueNew != null)
							//{
							//	await AddNewNode(nodeData, "");
							//	return;
							//}

							AddByMilliseconds(nodeData);
							AddByRange(nodeData);
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
