namespace QIA.Opc.Infrastructure.Managers;

using System.Collections.Concurrent;
using global::Opc.Ua.Client;
using MediatR;
using QIA.Opc.Application.Events;
using QIA.Opc.Application.Requests;
using QIA.Opc.Infrastructure.Application;
using QIA.Opc.Infrastructure.Entities;

public class SubscriptionManager
{
	//const	
	private const string SubscriptionKey = "subscriptionId=";

	private readonly SessionManager _sessionManager;
	private readonly IPublisher _publisher;

	//out delegates
	public delegate void SessionEventHandler(ISession session, NotificationEventArgs e);
	public event SessionEventHandler Session_NotificationEvent;

	//
	private readonly NotificationEventHandler _sessionNotification;
	private readonly PublishStateChangedEventHandler _publishStatusChanged;
	private readonly SubscriptionStateChangedEventHandler _subscriptionStateChanged;
	/// <summary>
	/// used to refer to database entity only <sessionId+subscriptionId, subscriptionGuid>
	/// </summary>
	private readonly ConcurrentDictionary<string, SubscriptionRequest> _subscriptions = new();

	public SubscriptionManager(SessionManager sessionManager, IPublisher publisher)
	{
		_sessionManager = sessionManager;
		_publisher = publisher;
		_sessionNotification = new NotificationEventHandler(Session_NotificationAsync);
		_publishStatusChanged = new PublishStateChangedEventHandler(Subscription_PublishStatusChanged);
		_subscriptionStateChanged = new SubscriptionStateChangedEventHandler(Subscription_StateChanged);
	}

	public Subscription Subscribe(string sessionNodeId, SubscriptionRequest subsParams, out OPCUASession session)
	{
		_sessionManager.TryGetSession(sessionNodeId, out session);
		Subscription subscription = CreateSubscription(session.Session, subsParams);

		_subscriptions.TryAdd(sessionNodeId + SubscriptionKey + subscription.Id.ToString(), subsParams);

		return subscription;
	}

	public bool DeleteSubscription(string sessionNodeId, uint subscriptionId, out string subscriptionGuid)
	{
		subscriptionGuid = "";
		_sessionManager.TryGetSession(sessionNodeId, out OPCUASession session);
		Subscription subscription = session.Session.Subscriptions.FirstOrDefault(c => c.Id == subscriptionId);

		if (subscription != null)
		{

			session.Session.RemoveSubscription(subscription);
			subscription.Delete(true);
			_subscriptions.TryGetValue(sessionNodeId + SubscriptionKey + subscriptionId.ToString(), out SubscriptionRequest subscriptionMemo);
			subscriptionGuid = subscriptionMemo.Guid;

			_subscriptions.TryRemove(sessionNodeId + SubscriptionKey + subscriptionId.ToString(), out _);
			return true;
		}

		return false;
	}

	public void Modify(string sessionNodeId, SubscriptionRequest subsParams)
	{
		_sessionManager.TryGetSession(sessionNodeId, out OPCUASession session);

		Subscription subscription = session.Session.Subscriptions.FirstOrDefault(s => s.Id == subsParams.OpcUaId);
		if (subscription == null)
		{
			return;//subscription is not active;
		}

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

	public Subscription SetPublishingMode(string sessionNodeId, uint subscriptionId, bool enable)
	{
		_sessionManager.TryGetSession(sessionNodeId, out OPCUASession session);

		Subscription subscription = session.Session.Subscriptions.FirstOrDefault(s => s.Id == subscriptionId);

		if (subscription == null)
		{
			return null;
		}

		subscription.SetPublishingMode(enable);
		return subscription;
	}

	public IEnumerable<OPCUASubscription> GetActiveOpcSubscriptions(string sessionNodeId, out string sessionGuid)
	{
		sessionGuid = "";
		List<OPCUASubscription> subscriptions = new();
		_sessionManager.TryGetSession(sessionNodeId, out OPCUASession session, true);
		if (session == null)
		{
			return subscriptions;
		}
		sessionGuid = session.Guid;

		foreach (Subscription item in session.Session.Subscriptions)
		{
			_subscriptions.TryGetValue(sessionNodeId + SubscriptionKey + item.Id.ToString(), out SubscriptionRequest subscription);
			var subscriptionGuid = subscription.Guid;

			//unexpected
			if (string.IsNullOrEmpty(subscriptionGuid))
			{
				_subscriptions.TryRemove(sessionNodeId + SubscriptionKey + item.Id.ToString(), out _);
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

	public Subscription GetSubscription(string sessionNodeId, uint subscriptionId, out SubscriptionRequest subscriptionRequest)
	{
		subscriptionRequest = null;
		_sessionManager.TryGetSession(sessionNodeId, out OPCUASession session);
		if (session == null)
		{
			return null;
		}
		_subscriptions.TryGetValue(sessionNodeId + SubscriptionKey + subscriptionId.ToString(), out subscriptionRequest);

		if (subscriptionRequest == null)
		{
			_subscriptions.TryRemove(sessionNodeId + SubscriptionKey + subscriptionId.ToString(), out _);
			throw new Exception("Subscription guid is empty");
		}

		return session.Session.Subscriptions.FirstOrDefault(c => c.Id == subscriptionId);
	}

	#region private

	private Subscription CreateSubscription(Session session, SubscriptionRequest subsParams)
	{
		Subscription subscription = new(session.DefaultSubscription)
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

		subscription.Session.Notification += _sessionNotification;
		subscription.StateChanged += _subscriptionStateChanged;
		subscription.PublishStatusChanged += _publishStatusChanged;
		LoggerManager.Logger.Information($"| Subscription {subscription.Id} created");

		return subscription;
	}

	#endregion

	#region events

	private void Session_NotificationAsync(ISession session, NotificationEventArgs e) => Session_NotificationEvent?.Invoke(session, e);

	private void Subscription_StateChanged(Subscription subscription, SubscriptionStateChangedEventArgs e)
	{
		Subscription_StateChangedEvent subscription_StateChangedEvent = new()
		{
			LogCategory = Qia.Opc.Domain.Entities.Enums.LogCategory.Warning,
			Message = $"{e.Status} publishing enabled: {subscription.PublishingEnabled}",
			Title = $"Subscription {subscription.Id} state",
			SessionId = subscription.Session.SessionId.ToString(),
		};
		_publisher.Publish(subscription_StateChangedEvent);

		LoggerManager.Logger.Information($"Subscription {subscription.Id} state: " + e.Status.ToString() + $" publishing enabled: {subscription.PublishingEnabled}");
	}

	private void Subscription_PublishStatusChanged(Subscription subscription, PublishStateChangedEventArgs e)
	{
		Subscription_PublishStatusChangedEvent subscription_PublishStatusChangedEvent = new()
		{
			LogCategory = Qia.Opc.Domain.Entities.Enums.LogCategory.Warning,
			Message = $"{e.Status} publishing enabled: {subscription.PublishingEnabled}",
			Title = $"Subscription {subscription.Id} state",
			SessionId = subscription.Session.SessionId.ToString(),
		};
		_publisher.Publish(subscription_PublishStatusChangedEvent);

		LoggerManager.Logger.Information($"Publish state {subscription.Id} : " + e.Status.ToString());
	}

	#endregion
}
