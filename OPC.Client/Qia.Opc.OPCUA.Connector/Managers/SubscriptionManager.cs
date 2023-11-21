using Opc.Ua;
using Opc.Ua.Client;
using Qia.Opc.Domain.Common;
using Qia.Opc.Domain.Core;
using Qia.Opc.Domain.Entities;
using QIA.Opc.Domain.Request;

namespace Qia.Opc.OPCUA.Connector.Managers
{
	public class SubscriptionManager
	{
		private readonly SessionManager sessionManager;
		private readonly NodeManager nodeManager;

		//delegates
		public delegate void SessionEventHandler(ISession session, NotificationEventArgs e);
		public event SessionEventHandler Session_NotificationEvent;

		public delegate void EventMessageHandler(object sender, EventData e);
		public event EventMessageHandler EventMessage;
		//
		private NotificationEventHandler m_SessionNotification = null;
		private PublishStateChangedEventHandler m_PublishStatusChanged = null;
		private SubscriptionStateChangedEventHandler m_SubscriptionStateChanged = null;

		public SubscriptionManager(SessionManager sessionManager, NodeManager nodeManager)
		{
			this.sessionManager = sessionManager;
			this.nodeManager = nodeManager;

			m_SessionNotification = new NotificationEventHandler(Session_NotificationAsync);
			m_PublishStatusChanged = new PublishStateChangedEventHandler(Subscription_PublishStatusChanged);
			m_SubscriptionStateChanged = new SubscriptionStateChangedEventHandler(Subscription_StateChanged);
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

		public uint Subscribe(SubscriptionParameters subsParams, string nodeId)
		{
			var subscription = CreateSubscription(subsParams);
			AddMonitoredItem(subscription, nodeId);
			return subscription.Id;
		}

		public void Modify(SubscriptionParameters subsParams, uint subscriptionId)
		{
			var session = sessionManager.CurrentSession.Session;

			Subscription subscription = session.Subscriptions.FirstOrDefault(s => s.Id == subscriptionId);
			if (subscription == null) return;

			subscription.DisplayName = subsParams.DisplayName;
			subscription.PublishingInterval = subsParams.PublishingInterval;
			subscription.KeepAliveCount = subsParams.KeepAliveCount;
			subscription.LifetimeCount = subsParams.LifetimeCount;
			subscription.MaxNotificationsPerPublish = subsParams.MaxNotificationsPerPublish;
			subscription.Priority = subsParams.Priority;
			subscription.PublishingEnabled = subsParams.PublishingEnabled;

			if (subscription.Created)
			{
				subscription.SetPublishingMode(subscription.PublishingEnabled);
			}

			subscription.Modify();
		}

		public void AddToSubscription(string subscriptionName, string nodeId)
		{
			var session = sessionManager.CurrentSession.Session;

			Subscription subscription = session.Subscriptions.FirstOrDefault(s => s.DisplayName == subscriptionName);
			if (subscription == null) return;
			AddMonitoredItem(subscription, nodeId);
		}


		public void SetPublishingMode(string subscriptionId, bool enable)
		{
			var session = sessionManager.CurrentSession.Session;

			Subscription subscription = session.Subscriptions.FirstOrDefault(s => s.Id.ToString() == subscriptionId);

			if (subscription != null)
			{
				subscription.SetPublishingMode(enable);
			}
		}

		public void DeleteMonitoringItem(uint subscriptionId, string nodeId)
		{
			var session = sessionManager.CurrentSession.Session;
			Subscription subscription = session.Subscriptions.FirstOrDefault(s => s.Id == subscriptionId);

			var monItem = subscription.MonitoredItems.FirstOrDefault(c => c.StartNodeId.ToString() == nodeId);
			subscription.RemoveItem(monItem);
			subscription.ApplyChanges();

		}

		private Subscription CreateSubscription(SubscriptionParameters subsParams)
		{
			var session = sessionManager.CurrentSession.Session;
			var subscription = new Subscription(session.DefaultSubscription)
			{
				DisplayName = subsParams.DisplayName,
				PublishingInterval = subsParams.PublishingInterval,
				KeepAliveCount = subsParams.KeepAliveCount,
				LifetimeCount = subsParams.LifetimeCount,
				MaxNotificationsPerPublish = subsParams.MaxNotificationsPerPublish,
				Priority = subsParams.Priority,
				PublishingEnabled = subsParams.PublishingEnabled,
			};

			try
			{
				Subscription duplicateSubscription2 = session.Subscriptions.FirstOrDefault(s => s.DisplayName == subsParams.DisplayName);
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
			LoggerManager.Logger.Information($"| Subscription {subscription.Id} created");

			return subscription;
		}

		private void AddMonitoredItem(Subscription subscription, string nodeId)
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

				var nodeReferenceDescription = sessionManager.CurrentSession.Session.Browse(null, null, 0u, new BrowseDescriptionCollection() { desc }, out var results, out var diagnosticInfos);

				if (nodeReferenceDescription == null)
				{
					LoggerManager.Logger.Error($"Node with ID {nodeId} not found.");
					return;
				}
				var nodeData = nodeManager.FindNodeOnServer(nodeId);

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
			}
			catch (Exception ex)
			{
				LoggerManager.Logger.Error("Failed to add to monitoring: {0}", ex.Message);
			}
		}


		#region events

		private void Session_NotificationAsync(ISession session, NotificationEventArgs e)
		{
			Session_NotificationEvent?.Invoke(session, e);
		}

		private void Subscription_StateChanged(Subscription subscription, SubscriptionStateChangedEventArgs e)
		{
			EventMessage?.Invoke(this, new EventData
			{
				LogCategory = Domain.Entities.Enums.LogCategory.Info,
				Message = e.Status.ToString(),
				Title = "Subscription state"
			});
			LoggerManager.Logger.Information(e.ToString());
		}

		private void Subscription_PublishStatusChanged(Subscription subscription, PublishStateChangedEventArgs e)
		{
			EventMessage?.Invoke(this, new EventData
			{
				LogCategory = Domain.Entities.Enums.LogCategory.Info,
				Message = e.Status.ToString(),
				Title = "Publish state"
			});
			LoggerManager.Logger.Information(e.ToString());
		}
		#endregion

	}
}
