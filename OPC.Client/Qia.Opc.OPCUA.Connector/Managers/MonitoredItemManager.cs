using Opc.Ua;
using Opc.Ua.Client;

namespace Qia.Opc.OPCUA.Connector.Managers
{
	public class MonitoredItemManager
	{
		private readonly SessionManager _sessionManager;

		public MonitoredItemManager(SessionManager sessionManager)
		{
			_sessionManager = sessionManager;
		}

		public MonitoredItem CreateMonitoredItem(string nodeId)
		{
			var opcSession = _sessionManager.CurrentSession;
			var session = opcSession.Session as Session;

			if (session == null)
			{
				throw new ArgumentException("Invalid session ID.");
			}


			var desc = new BrowseDescription
			{
				NodeId = NodeId.Parse(nodeId),
				BrowseDirection = BrowseDirection.Forward,
				IncludeSubtypes = true,
				NodeClassMask = (uint)NodeClass.Variable | (uint)NodeClass.Object,
				ResultMask = (uint)BrowseResultMask.All
			};

			var node = session.Browse(null, null, 0u, new BrowseDescriptionCollection() { desc }, out var results, out var diagnosticInfos);

			if (node == null)
			{
				throw new ArgumentException($"Node with ID {nodeId} not found.");
			}

			var subscription = new Subscription(session.DefaultSubscription);
			subscription.Create();

			var monitoredItem = new MonitoredItem(subscription.DefaultItem)
			{
				//StartNodeId = node.NodeId,
				//AttributeId = Attributes.Value,
				//DisplayName = node.DisplayName.Text,
				//SamplingInterval = 1000,

			};
			monitoredItem.Notification += OnMonitoredItemNotification;

			subscription.AddItem(monitoredItem);
			subscription.ApplyChanges();
			session.AddSubscription(subscription);

			return monitoredItem;
		}

		public void DeleteMonitoredItem(uint subscriptionId, uint monitoredItemId)
		{
			var session = _sessionManager.CurrentSession.Session;

			Subscription subscription = session.Subscriptions.FirstOrDefault(c => c.Id == subscriptionId);

			MonitoredItem item = subscription.FindItemByClientHandle(monitoredItemId);
			if (item != null)
			{
				subscription.RemoveItem(item);
				subscription.ApplyChanges();
			}
		}

		public IEnumerable<MonitoredItem> GetAllMonitoredItems()
		{
			var session = _sessionManager.CurrentSession;
			var items = new List<MonitoredItem>();
			foreach (var subscription in session.Session.Subscriptions)
			{
				items.AddRange(subscription.MonitoredItems);
			}
			return items;
		}

		private void OnMonitoredItemNotification(MonitoredItem item, MonitoredItemNotificationEventArgs e)
		{
		}
	}
}
