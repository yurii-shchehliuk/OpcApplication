using Opc.Ua.Client;
using Qia.Opc.OPCUA.Connector.Entities;
using QIA.Opc.Domain.Common;
using QIA.Opc.Domain.Entities;
using QIA.Opc.Domain.Requests;
using System.Collections.Concurrent;

namespace Qia.Opc.OPCUA.Connector.Managers
{
	public class SubscriptionManager
	{
		//const	
		private const string subscriptionKey = "subscriptionId=";
		private readonly SessionManager sessionManager;

		//delegates
		public delegate void SessionEventHandler(ISession session, NotificationEventArgs e);
		public event SessionEventHandler Session_NotificationEvent;

		public delegate void EventMessageHandler(object sender, EventData e);
		public event EventMessageHandler EventMessage;
		//
		private NotificationEventHandler m_SessionNotification = null;
		private PublishStateChangedEventHandler m_PublishStatusChanged = null;
		private SubscriptionStateChangedEventHandler m_SubscriptionStateChanged = null;
		/// <summary>
		/// used to refer to database entity only <sessionId+subscriptionId, subscriptionGuid>
		/// </summary>
		private ConcurrentDictionary<string, string> _subscriptions = new ConcurrentDictionary<string, string>();

		public SubscriptionManager(SessionManager sessionManager)
		{
			this.sessionManager = sessionManager;

			m_SessionNotification = new NotificationEventHandler(Session_NotificationAsync);
			m_PublishStatusChanged = new PublishStateChangedEventHandler(Subscription_PublishStatusChanged);
			m_SubscriptionStateChanged = new SubscriptionStateChangedEventHandler(Subscription_StateChanged);
		}

		public Subscription Subscribe(string sessionNodeId, SubscriptionRequest subsParams, out OPCUASession session)
		{
			sessionManager.TryGetSession(sessionNodeId, out session);
			var subscription = CreateSubscription(session.Session, subsParams);

			_subscriptions.TryAdd(sessionNodeId + subscriptionKey + subscription.Id.ToString(), subsParams.Guid);

			return subscription;
		}

		public bool DeleteSubscription(string sessionNodeId, uint subscriptionId, out string subscriptionGuid)
		{
			subscriptionGuid = "";
			sessionManager.TryGetSession(sessionNodeId, out var session);
			Subscription subscription = session.Session.Subscriptions.FirstOrDefault(c => c.Id == subscriptionId);

			if (subscription != null)
			{

				session.Session.RemoveSubscription(subscription);
				subscription.Delete(true);
				_subscriptions.TryGetValue(sessionNodeId + subscriptionKey + subscriptionId.ToString(), out subscriptionGuid);
				_subscriptions.TryRemove(sessionNodeId + subscriptionKey + subscriptionId.ToString(), out _);
				return true;
			}

			return false;
		}

		public void Modify(string sessionNodeId, SubscriptionRequest subsParams, out string subscriptionGuid)
		{
			subscriptionGuid = "";
			sessionManager.TryGetSession(sessionNodeId, out var session);

			Subscription subscription = session.Session.Subscriptions.FirstOrDefault(s => s.Id == subsParams.OpcUaId);
			if (subscription == null) return;//subscription is not active;

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
			_subscriptions.TryGetValue(sessionNodeId + subscriptionKey + subsParams.OpcUaId.ToString(), out subscriptionGuid);
		}

		public Subscription SetPublishingMode(string sessionNodeId, uint subscriptionId, bool enable)
		{
			sessionManager.TryGetSession(sessionNodeId, out var session);

			Subscription subscription = session.Session.Subscriptions.FirstOrDefault(s => s.Id == subscriptionId);

			if (subscription == null)
				return null;

			subscription.SetPublishingMode(enable);
			return subscription;
		}

		public IEnumerable<OPCUASubscription> GetActiveOpcSubscriptions(string sessionNodeId, out string sessionGuid)
		{
			sessionGuid = "";
			var subscriptions = new List<OPCUASubscription>();
			sessionManager.TryGetSession(sessionNodeId, out var session, true);
			if (session == null)
			{
				return subscriptions;
			}
			sessionGuid = session.Guid;

			foreach (var item in session.Session.Subscriptions)
			{
				_subscriptions.TryGetValue(sessionNodeId + subscriptionKey + item.Id.ToString(), out string subscriptionGuid);

				//unexpected
				if (string.IsNullOrEmpty(subscriptionGuid))
				{
					_subscriptions.TryRemove(sessionNodeId + subscriptionKey + item.Id.ToString(), out _);
					continue;
				}
				subscriptions.Add(new OPCUASubscription
				{
					Subscription = item,
					Guid = subscriptionGuid
				});
			};

			return subscriptions;
		}

		public Subscription GetSubscription(string sessionNodeId, uint subscriptionId, out string subscriptionGuid)
		{
			subscriptionGuid = "";
			sessionManager.TryGetSession(sessionNodeId, out var session);
			if (session == null)
			{
				return null;
			}
			_subscriptions.TryGetValue(sessionNodeId + subscriptionKey + subscriptionId.ToString(), out subscriptionGuid);
			if (string.IsNullOrEmpty(subscriptionGuid))
			{
				_subscriptions.TryRemove(sessionNodeId + subscriptionKey + subscriptionId.ToString(), out _);
			}

			return session.Session.Subscriptions.FirstOrDefault(c => c.Id == subscriptionId);
		}

		#region private

		private Subscription CreateSubscription(Session session, SubscriptionRequest subsParams)
		{
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

		#endregion

		#region events

		private void Session_NotificationAsync(ISession session, NotificationEventArgs e)
		{
			Session_NotificationEvent?.Invoke(session, e);
		}

		private void Subscription_StateChanged(Subscription subscription, SubscriptionStateChangedEventArgs e)
		{
			LoggerManager.Logger.Information($"Subscription {subscription.Id} state: " + e.Status.ToString() + $" publishing enabled: {subscription.PublishingEnabled}");
		}

		private void Subscription_PublishStatusChanged(Subscription subscription, PublishStateChangedEventArgs e)
		{
			LoggerManager.Logger.Information($"Publish state {subscription.Id} : " + e.Status.ToString());
		}

		#endregion
	}
}
