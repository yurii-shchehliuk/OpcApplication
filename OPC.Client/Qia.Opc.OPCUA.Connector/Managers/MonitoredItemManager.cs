using Opc.Ua;
using Opc.Ua.Client;
using Qia.Opc.OPCUA.Connector.Managers;
using QIA.Opc.Domain.Common;
using QIA.Opc.Domain.Requests;

namespace QIA.Opc.OPCUA.Connector.Managers
{
	public class MonitoredItemManager
	{
		private readonly SessionManager sessionManager;

		public MonitoredItemManager(SessionManager sessionManager)
		{
			this.sessionManager = sessionManager;
		}

		public MonitoredItem AddToSubscription(string sessionGuid, uint subscriptionId, string nodeId)
		{
			sessionManager.TryGetSession(sessionGuid, out var session);

			Subscription subscription = session.Session.Subscriptions.FirstOrDefault(s => s.Id == subscriptionId);
			if (subscription == null) return null;

			return AddMonitoredItem(session.Session, subscription, nodeId);
		}

		public MonitoredItem AddToSubscription(string sessionGuid, Subscription subscription, string nodeId)
		{
			sessionManager.TryGetSession(sessionGuid, out var session);

			return AddMonitoredItem(session.Session, subscription, nodeId);
		}

		public MonitoredItem GetMonitoringItem(string sessionGuid, uint subscriptionId, string nodeId)
		{
			sessionManager.TryGetSession(sessionGuid, out var session);
			Subscription subscription = session.Session.Subscriptions.FirstOrDefault(s => s.Id == subscriptionId);
			if (subscription == null) return null;

			var monItem = subscription.MonitoredItems.FirstOrDefault(c => c.StartNodeId.ToString() == nodeId);

			return monItem;
		}

		public MonitoredItem DeleteMonitoringItem(string sessionGuid, uint subscriptionId, string nodeId)
		{
			sessionManager.TryGetSession(sessionGuid, out var session);
			Subscription subscription = session.Session.Subscriptions.FirstOrDefault(s => s.Id == subscriptionId);
			if (subscription == null) return null;

			var monItem = subscription.MonitoredItems.FirstOrDefault(c => c.StartNodeId.ToString() == nodeId);
			subscription.RemoveItem(monItem);
			subscription.ApplyChanges();
			return monItem;
		}

		public bool UpdateMonitoredItem(string sessionGuid, MonitoredItemRequest updatedItem, uint subscriptionId)
		{
			sessionManager.TryGetSession(sessionGuid, out var session);

			Subscription subscription = session.Session.Subscriptions.FirstOrDefault(s => s.Id == subscriptionId);
			if (subscription == null) return false;

			var monItem = subscription.MonitoredItems.FirstOrDefault(c => c.StartNodeId.ToString() == updatedItem.StartNodeId);

			if (monItem == null) return false;

			monItem.DisplayName = updatedItem.DisplayName;
			monItem.SamplingInterval = updatedItem.SamplingInterval;
			monItem.QueueSize = updatedItem.QueueSize;
			monItem.DiscardOldest = updatedItem.DiscardOldest;
			// readonly
			//monItem.StartNodeId = updatedItem.StartNodeId;
			//monItem.RelativePath = updatedItem.RelativePath;
			//monItem.NodeClass = updatedItem.NodeClass;
			//monItem.AttributeId = updatedItem.AttributeId;
			//monItem.IndexRange = updatedItem.IndexRange;
			//monItem.MonitoringMode = updatedItem.MonitoringMode;

			subscription.ApplyChanges();

			return true;
		}

		private MonitoredItem AddMonitoredItem(Session session, Subscription subscription, string nodeId)
		{
			try
			{
				var opcNodeId = new NodeId(nodeId);
				var desc = new BrowseDescription
				{
					NodeId = opcNodeId,
					BrowseDirection = BrowseDirection.Forward,
					IncludeSubtypes = true,
					NodeClassMask = (uint)NodeClass.Variable | (uint)NodeClass.Object,
					ResultMask = (uint)BrowseResultMask.All
				};

				var nodeReferenceDescription = session.Browse(null, null, 0u, new BrowseDescriptionCollection() { desc }, out var results, out var diagnosticInfos);

				if (nodeReferenceDescription == null)
				{
					LoggerManager.Logger.Error($"Node with ID {nodeId} not found.");
					return null;
				}
				var nodeData = FindNodeOnServer(session, nodeId);

				MonitoredItem monitoredItem = new MonitoredItem(subscription.DefaultItem)
				{
					DisplayName = nodeData.DisplayName.ToString(),
					StartNodeId = opcNodeId,
					NodeClass = nodeData.NodeClass,
					AttributeId = Attributes.Value,
					SamplingInterval = 0,
					QueueSize = 1,
					DiscardOldest = true,
				};

				// add condition fields to any event filter.
				EventFilter filter = monitoredItem.Filter as EventFilter;

				if (filter != null)
				{
					monitoredItem.AttributeId = Attributes.EventNotifier;
					monitoredItem.QueueSize = 0;
				}

				//monitoredItem.Notification += (OnMonitoredItemNotification);

				subscription.AddItem(monitoredItem);
				subscription.ApplyChanges();
				LoggerManager.Logger.Information($"| {monitoredItem.StartNodeId} added to monitoring");

				return monitoredItem;
			}
			catch (Exception ex)
			{
				LoggerManager.Logger.Error("Failed to add to monitoring: {0}", ex.Message);
				throw;
			}
		}

		private Node FindNodeOnServer(Session session, string nodeId)
		{

			NodeId nodeIdToSearch = new NodeId(nodeId);

			Node node = session.ReadNode(nodeIdToSearch);

			return node;
		}

		private DataValue ReadValue(Session session, string nodeId)
		{
			try
			{
				DataValue nodeValue = session.ReadValue(nodeId);
				return nodeValue;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}
}
