using Microsoft.EntityFrameworkCore;
using Opc.Ua;
using Opc.Ua.Client;
using QIA.Plugin.OpcClient.Core;
using QIA.Plugin.OpcClient.Entities;
using QIA.Plugin.OpcClient.Repository;
using QIA.Plugin.OpcClient.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QIA.Plugin.OpcClient.Services
{
	using static LoggerManager;

	public class SubscriptionManager : ISubscriptionManager
	{
		private readonly INodeManager _nm;
		private readonly IDataAccess<NodeData> _repo;
		private readonly IAzureMessageService _azureMS;
		private readonly SignalRService signalRService;
		// core
		private Session m_session;
		private Subscription m_subscription;
		// events handler
		private NotificationEventHandler m_SessionNotification = null;
		private EventHandler m_PublishStatusChanged = null;
		private SubscriptionStateChangedEventHandler m_SubscriptionStateChanged = null;
		// events 
		public delegate Task TreeManagerHandler(object sender, NewNode node);
		public event TreeManagerHandler FindNode;

		public SubscriptionManager(INodeManager nm, IDataAccess<NodeData> repo, IAzureMessageService azureMessageService, SignalRService signalRService)
		{
			_nm = nm;
			_repo = repo;
			_azureMS = azureMessageService;
			this.signalRService = signalRService;
			signalRService.NodeMonitor += OnMessageReceived;

			m_session = OpcConfiguration.OpcUaClientSession;
			m_SessionNotification = new NotificationEventHandler(Session_NotificationAsync);
			try
			{
				//itemsToFind = JsonSerializer.Deserialize<HashSet<BaseNode>>(File.ReadAllText(".\\nodemanager.json"));
			}
			catch (Exception ex)
			{
				Logger.Error(ex, "Couldn't deserialize nodemanager");
			}
		}

		/// <summary>
		/// Event from signalR to handle the node.
		/// </summary>
		/// <remarks>To manage a subscription, it should have a reference description. So we have to trigger TreeManager to make a request to the server to find the RD</remarks>
		private void OnMessageReceived(object sender, NewNode message)
		{
			if (message.Action == MonitorAction.Monitor)
			{
				itemsToFind = new HashSet<BaseNode>(_repo.GetConfigNodes(message.Group).ToList());

				FindNode.Invoke(this, message);
				return;
			}
			// unmonitor
			m_session = OpcConfiguration.OpcUaClientSession;
			if (m_session.Subscriptions is null)
				return;

			foreach (var subscriptionItems in m_session.Subscriptions)
			{
				foreach (var monitoringItem in subscriptionItems.MonitoredItems)
				{
					var monitoringNodeId = monitoringItem.ResolvedNodeId.Identifier.ToString();
					if (monitoringNodeId == message.NodeId)
					{
						subscriptionItems.Delete(true);
					}
				}
			}
		}

		public void SubscribeNew(ReferenceDescription reference)
		{
			if (m_session == null)
			{
				//TODO
				m_session = OpcConfiguration.OpcUaClientSession;

			}

			if (reference == null || m_session == null)
				return;

			try
			{
				Subscription newSubscription = new Subscription(m_session.DefaultSubscription);
				m_session.AddSubscription(newSubscription);
				newSubscription.Create();

				Subscription duplicateSubscription = m_session.Subscriptions.FirstOrDefault(s => s.Id != 0 && s.Id.Equals(newSubscription.Id) && s != newSubscription);
				if (duplicateSubscription != null)
				{
					LoggerManager.Logger.Information("Duplicate subscription was created with the id: {0}", duplicateSubscription.Id);

					duplicateSubscription.Delete(false);
					m_session.RemoveSubscription(newSubscription);
				}

				if (m_subscription != null)
				{
					// remove previous subscription.
					m_subscription.StateChanged -= m_SubscriptionStateChanged;
					m_subscription.PublishStatusChanged -= m_PublishStatusChanged;
					m_subscription.Session.Notification -= m_SessionNotification;
				}
				if (newSubscription != null)
				{
					m_subscription = newSubscription;

					newSubscription.StateChanged += m_SubscriptionStateChanged;
					//m_subscription.PublishStatusChanged += m_PublishStatusChanged;
					newSubscription.Session.Notification += m_SessionNotification;

					//UpdateStatus();
					AddMonitoredItem(reference);
					LoggerManager.Logger.Information("Item added to monitoring");
				}
			}
			catch (Exception exception)
			{
				Logger.Error(exception.Message, exception);
				throw;
			}
		}

		public void SubscribeNew(string nodeId)
		{
			var node = _nm.GetNodeIdFromId(nodeId);
			_nm.ReadNodeValue(node);
			_nm.ReadNode(node);
			_nm.FindInCache(nodeId);
			SubscribeNew(reference: null);
		}

		private void AddMonitoredItem(ReferenceDescription reference)
		{
			try
			{
				MonitoredItem monitoredItem = new MonitoredItem(m_subscription.DefaultItem);

				monitoredItem.DisplayName = reference.DisplayName.Text;
				monitoredItem.StartNodeId = (NodeId)reference.NodeId;
				monitoredItem.NodeClass = reference.NodeClass;
				monitoredItem.AttributeId = Attributes.Value;
				monitoredItem.SamplingInterval = 0;
				monitoredItem.QueueSize = 1;

				// add condition fields to any event filter.
				EventFilter filter = monitoredItem.Filter as EventFilter;

				if (filter != null)
				{
					monitoredItem.AttributeId = Attributes.EventNotifier;
					monitoredItem.QueueSize = 0;
				}

				//monitoredItem.Notification += new MonitoredItemNotificationEventHandler(MonitoredItem_Notification);

				m_subscription.AddItem(monitoredItem);
				m_subscription.ApplyChanges();
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to add to monitoring: {0}", ex.Message);
				throw;
			}
		}

		/// <summary>
		/// avoid stackoverflow, skip first event
		/// </summary>
		private bool isHandlingEvent = true;
		private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

		private async void Session_NotificationAsync(Session session, NotificationEventArgs e)
		{
			//TODO: configure server events listening.
			if (isHandlingEvent)
			{
				isHandlingEvent = false;
				return;
			}
			isHandlingEvent = true;

			await _semaphore.WaitAsync();
			try
			{
				if (itemsToFind is null)
					itemsToFind = new HashSet<BaseNode>(await _repo.GetConfigNodes("").ToListAsync());

				await HandleSessionItems();
			}
			catch (Exception ex)
			{
				Logger.Error(ex.Message, ex);
			}
			finally
			{
				Thread.Sleep(1200);
				_semaphore.Release();
				m_SessionNotification.Invoke(m_session, e);
				isHandlingEvent = false;
			}
		}

		private HashSet<BaseNode> itemsToFind = null;

		private async Task HandleSessionItems()
		{
			foreach (var subscriptionItems in m_session.Subscriptions)
			{
				foreach (var monitoringItem in subscriptionItems.MonitoredItems)
				{
					string currentNodeId = monitoringItem.ResolvedNodeId.Identifier.ToString();
					var nodeConfig = NodeToFind(currentNodeId);

					if (nodeConfig != null)
					{
						var nodeValueNew = _nm.ReadNodeValue(monitoringItem.ResolvedNodeId);
						var node = await _repo.FindNodeByIdAsync(nodeConfig.NodeId);

						if (node == null && nodeValueNew != null)
						{
							await AddNewNode(nodeConfig.NodeId, nodeValueNew, "");
							return;
						}

						await AddByMilliseconds(node, nodeValueNew, nodeConfig);
						await AddByRange(node, nodeValueNew, nodeConfig);
					}
				}
			}
		}
		private BaseNode NodeToFind(NodeId node)
		{
			return itemsToFind.FirstOrDefault(c => c.NodeId == node.Identifier.ToString());
		}

		private void UpdateNodeValue(NodeData node, string nodeValueNew)
		{
			var nodeDto = NodeToFind(node.NodeId);
			node.Value = nodeValueNew;
			node.MSecs = string.IsNullOrEmpty(nodeValueNew) ? null : nodeDto.MSecs;
			node.Range = string.IsNullOrEmpty(nodeValueNew) ? null : nodeDto.Range;
			node.StoreTime = string.IsNullOrEmpty(nodeValueNew) ? null : DateTime.UtcNow;
			_repo.UpdateAsync(node);
		}
		List<NodeData> tempNodes = new();

		private async Task AddByMilliseconds(NodeData node, string nodeValueNew, BaseNode nodeConfig)
		{
			if (nodeConfig.MSecs == 0 || node.StoreTime == null)
				return;

			var diff = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - new DateTimeOffset((DateTime)node.StoreTime).ToUnixTimeMilliseconds();

			if (nodeConfig.MSecs > 0 && diff >= nodeConfig.MSecs)
			{
				await AddNewNode(nodeConfig.NodeId, nodeValueNew, "msecs");
				//await UpdateNodeValue(node, nodeValueNew);
			}
		}

		private async Task AddByRange(NodeData node, string nodeValueNew, BaseNode nodeConfig)
		{
			if (nodeConfig.Range == 0)
				return;

			int tempDbCount = tempNodes.Where(c => c.NodeId == nodeConfig.NodeId).Count();

			// update node per range
			if (tempDbCount < nodeConfig.Range)
			{
				var nodeDto = NodeToFind(node.NodeId);

				tempNodes.Add(new NodeData(nodeDto, nodeValueNew));
			}
			else
			{
				//await UpdateNodeValue(node, nodeValueNew);
				await AddNewNode(nodeConfig.NodeId, nodeValueNew, "range");
				tempNodes.RemoveAll(c => c.NodeId == nodeConfig.NodeId);
			}
		}

		private async Task AddNewNode(string currentNodeId, string nodeValueNew, string monitorCategory)
		{
			var nodeDto = NodeToFind(currentNodeId);
			NodeData entity = new(nodeDto, nodeValueNew);

			Logger.Information($"[monitoring {monitorCategory}] {nodeDto.Name} {nodeValueNew} {DateTimeOffset.UtcNow}");
			_repo.AddMonitoringValue(entity).Wait();
			await signalRService.SendNodeAction(entity);
			await _azureMS.SendNodeAsync(entity);
		}
	}
}
