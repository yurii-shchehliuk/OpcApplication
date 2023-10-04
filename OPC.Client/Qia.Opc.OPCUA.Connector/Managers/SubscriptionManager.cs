using Opc.Ua;
using Opc.Ua.Client;
using Qia.Opc.Domain.Core;
using Qia.Opc.Domain.Entities;

namespace Qia.Opc.OPCUA.Connector.Managers
{
	public class SubscriptionManager
	{
		private readonly SessionManager sessionManager;
		private readonly NodeManager nodeManager;

		//delegates
		public delegate void NodeMonitorUpdateHandler(object sender, MonitoredItem node);
		public event NodeMonitorUpdateHandler NodeMonitorUpdate;
		public delegate void SessionEventHandler(object session, NotificationEventArgs e);
		public event SessionEventHandler SessionEvent;
		//
		private NotificationEventHandler m_SessionNotification = null;
		private EventHandler m_PublishStatusChanged = null;
		private SubscriptionStateChangedEventHandler m_SubscriptionStateChanged = null;
		private MonitoredItemNotificationEventHandler m_MonitoredItemNotification;
		public SubscriptionManager(SessionManager sessionManager, NodeManager nodeManager)
		{
			this.sessionManager = sessionManager;
			this.nodeManager = nodeManager;

			m_SessionNotification= new NotificationEventHandler(Session_NotificationAsync);
			m_PublishStatusChanged = new EventHandler(PublishStatusChanged);
			m_SubscriptionStateChanged = new SubscriptionStateChangedEventHandler(SubscriptionStateChanged);
			m_MonitoredItemNotification = new MonitoredItemNotificationEventHandler(OnMonitoredItemNotification);
		}

		public bool DeleteSubscription(uint subscriptionId)
		{
			var session = sessionManager.CurrentSession.Session;
			Subscription subscription = session.Subscriptions.FirstOrDefault(c => c.Id == subscriptionId);

			if (subscription != null)
			{
				session.RemoveSubscription(subscription);
				subscription.Delete(true);
				return true;
			}

			return false;
		}

		public IEnumerable<Subscription> GetActiveSubscriptions()
		{
			return sessionManager.CurrentSession.Session.Subscriptions;
		}

		public IEnumerable<MonitoredItem> GetAllMonitoredItems()
		{
			var session = sessionManager.CurrentSession;
			var items = new List<MonitoredItem>();
			foreach (var subscription in session.Session.Subscriptions)
			{
				items.AddRange(subscription.MonitoredItems);
			}
			return items;
		}

		public void Subscribe(NodeReferenceEntity nodeRef)
		{
			var subscription = CreateSubscription(nodeRef.DisplayName);
			AddMonitoredItem(subscription, nodeRef);
		}

		private Subscription CreateSubscription(string displayName)
		{
			var session = sessionManager.CurrentSession.Session;
			Subscription subscription = new Subscription(session.DefaultSubscription)
			{
				DisplayName = displayName
			};
			try
			{
				session.AddSubscription(subscription);
				subscription.Create();

				Subscription duplicateSubscription = session.Subscriptions.FirstOrDefault(s => s.Id != 0 && s.Id.Equals(subscription.Id) && s != subscription);
				if (duplicateSubscription != null)
				{
					LoggerManager.Logger.Information("Duplicate subscription was created with the id: {0}", duplicateSubscription.Id);

					duplicateSubscription.Delete(false);
					session.RemoveSubscription(subscription);
				}
			}
			catch (Exception ex)
			{
				LoggerManager.Logger.Error(ex.Message + "{0}", ex);
			}

			subscription.Session.Notification += m_SessionNotification;
			subscription.StateChanged += m_SubscriptionStateChanged;
			//m_subscription.PublishStatusChanged += m_PublishStatusChanged;

			return subscription;
		}

		private void AddMonitoredItem(Subscription subscription, Domain.Entities.NodeReferenceEntity reference)
		{
			try
			{
				var nodeId = new NodeId(reference.NodeId);
				var desc = new BrowseDescription
				{
					NodeId = nodeId,
					BrowseDirection = BrowseDirection.Forward,
					IncludeSubtypes = true,
					NodeClassMask = (uint)NodeClass.Variable | (uint)NodeClass.Object,
					ResultMask = (uint)BrowseResultMask.All
				};

				var node = sessionManager.CurrentSession.Session.Browse(null, null, 0u, new BrowseDescriptionCollection() { desc }, out var results, out var diagnosticInfos);
				var t = nodeManager.FindNode(reference.NodeId);

				if (node == null)
				{
					LoggerManager.Logger.Error($"Node with ID {reference.NodeId} not found.");
					return;
				}

				MonitoredItem monitoredItem = new MonitoredItem(subscription.DefaultItem)
				{
					DisplayName = reference.DisplayName,
					StartNodeId = nodeId,
					NodeClass = reference.NodeClass,
					AttributeId = Attributes.Value,
					SamplingInterval = 0,
					QueueSize = Int32.MaxValue,
					DiscardOldest = true,
				};

				// add condition fields to any event filter.
				EventFilter filter = monitoredItem.Filter as EventFilter;

				if (filter != null)
				{
					monitoredItem.AttributeId = Attributes.EventNotifier;
					monitoredItem.QueueSize = 0;
				}

				monitoredItem.Notification += (OnMonitoredItemNotification);

				subscription.AddItem(monitoredItem);
				subscription.ApplyChanges();
				LoggerManager.Logger.Information("Item added to monitoring");
			}
			catch (Exception ex)
			{
				LoggerManager.Logger.Error("Failed to add to monitoring: {0}", ex.Message);
			}
		}

		private void OnMonitoredItemNotification(MonitoredItem item, MonitoredItemNotificationEventArgs e)
		{
			try
			{
				// notify controls of the change.
				if (e != null)
				{
					MonitoredItemNotification notification = e.NotificationValue as MonitoredItemNotification;
				}
				// update item status.
			}
			catch (Exception exception)
			{
			}
			NodeMonitorUpdate.Invoke(this, item);
		}

		private void Session_NotificationAsync(ISession session, NotificationEventArgs e)
		{
			SessionEvent.Invoke(session, e);
		}

		private void SubscriptionStateChanged(Subscription subscription, SubscriptionStateChangedEventArgs e)
		{
			LoggerManager.Logger.Information(e.ToString());
		}

		private void PublishStatusChanged(object? sender, EventArgs e)
		{
			LoggerManager.Logger.Information(e.ToString());
		}

	}
}
