using Opc.Ua;
using Opc.Ua.Client;
using Qia.Opc.Domain.Common;
using Qia.Opc.Domain.Core;
using Qia.Opc.Domain.Entities;

namespace Qia.Opc.OPCUA.Connector.Managers
{
	public class SubscriptionManager
	{
		private readonly SessionManager sessionManager;
		private readonly NodeManager nodeManager;

		//delegates
		public delegate void NodeMonitorUpdateHandler(object sender, NodeValue node);
		public event NodeMonitorUpdateHandler NodeMonitorUpdate;

		public delegate void SessionEventHandler(ISession session, NotificationEventArgs e);
		public event SessionEventHandler SessionEvent;

		public delegate void EventMessageHandler(object sender, EventData e);
		public event EventMessageHandler EventMessage;
		//
		private NotificationEventHandler m_SessionNotification = null;
		private PublishStateChangedEventHandler m_PublishStatusChanged = null;
		private SubscriptionStateChangedEventHandler m_SubscriptionStateChanged = null;
		private MonitoredItemNotificationEventHandler m_MonitoredItemNotification = null;
		public SubscriptionManager(SessionManager sessionManager, NodeManager nodeManager)
		{
			this.sessionManager = sessionManager;
			this.nodeManager = nodeManager;

			m_SessionNotification = new NotificationEventHandler(Session_NotificationAsync);
			m_PublishStatusChanged = new PublishStateChangedEventHandler(PublishStatusChanged);
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

		public NodeReferenceEntity Subscribe(NodeReferenceEntity nodeRef)
		{
			var subscription = CreateSubscription(nodeRef.DisplayName);
			nodeRef.SubscriptionId = subscription.Id.ToString();
			AddMonitoredItem(subscription, nodeRef);
			return nodeRef;
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
				Subscription duplicateSubscription2 = session.Subscriptions.FirstOrDefault(s => s.DisplayName == displayName);
				if (duplicateSubscription2 != null)
				{
					LoggerManager.Logger.Information("Duplicate subscription with the same name: {0}", duplicateSubscription2.DisplayName);
					session.RemoveSubscription(subscription);
				}

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
			subscription.PublishStatusChanged += m_PublishStatusChanged;

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
				var t = nodeManager.FindNodeOnServer(reference.NodeId);

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
			if (e != null)
			{
				MonitoredItemNotification notification = e.NotificationValue as MonitoredItemNotification;


				NodeValue node = new NodeValue()
				{
					NodeId = item.StartNodeId.ToString(),
					Value = notification.Value.ToString(),
				};
				NodeMonitorUpdate.Invoke(this, node);
			}
		}

		private void Session_NotificationAsync(ISession session, NotificationEventArgs e)
		{
			SessionEvent.Invoke(session, e);
		}

		private void SubscriptionStateChanged(Subscription subscription, SubscriptionStateChangedEventArgs e)
		{
			EventMessage?.Invoke(this, new EventData
			{
				LogCategory = Domain.Entities.Enums.LogCategory.Info,
				Message = e.Status.ToString(),
				Title = "Subscription state"
			});
			LoggerManager.Logger.Information(e.ToString());
		}

		private void PublishStatusChanged(Subscription subscription, PublishStateChangedEventArgs e)
		{
			EventMessage?.Invoke(this, new EventData
			{
				LogCategory = Domain.Entities.Enums.LogCategory.Info,
				Message = e.Status.ToString(),
				Title = "Publish state"
			});
			LoggerManager.Logger.Information(e.ToString());
		}
	}
}
