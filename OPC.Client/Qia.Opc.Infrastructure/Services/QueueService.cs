namespace QIA.Opc.Infrastructure.Services;

using global::Opc.Ua;
using global::Opc.Ua.Client;
using Qia.Opc.Domain.Entities;
using QIA.Opc.Application.Responses;
using QIA.Opc.Infrastructure.Application;
using QIA.Opc.Infrastructure.Managers;

public class QueueService
{
    internal static bool IsHandlingEvent;

    private NotificationEventArgs notificationEvent;

    private readonly SessionManager _sessionManager;
    private readonly SubscriptionManager _subscriptionManager;
    private readonly UniqueQueue<NotificationMessage> _uniqueNotification;

    public QueueService(SessionManager sessionManager, SubscriptionManager subscriptionManager, UniqueQueue<NotificationMessage> uniqueNotification)
    {
        _sessionManager = sessionManager;
        _subscriptionManager = subscriptionManager;
        _uniqueNotification = uniqueNotification;
    }

    public List<MonitoredItemNotification> GetChanges(NotificationEventArgs e)
    {
        notificationEvent = e;
        if (IsHandlingEvent)
        {
            IsHandlingEvent = false;
            return null;
        }

        if (_uniqueNotification.Contains(notificationEvent.NotificationMessage))
        {
            return null;
        }
        IsHandlingEvent = true;
        _uniqueNotification.Enqueue(notificationEvent.NotificationMessage);

        // get the changes.
        List<MonitoredItemNotification> changes = new();
        foreach (MonitoredItemNotification change in e.NotificationMessage.GetDataChanges(false))
        {
            if (e.Subscription.FindItemByClientHandle(change.ClientHandle) == null)
            {
                continue;
            }
            changes.Add(change);
        }

        if (changes.Count == 0
            || e.Subscription.Session.SessionId == null)
        {
            return null;
        }

        return changes;

    }

    public SubscriptionValue GetSubscription(List<MonitoredItemNotification> changes)
    {
        var eventDetails = $"{notificationEvent.Subscription.Session.SessionId}:{notificationEvent.NotificationMessage.SequenceNumber}:{notificationEvent.NotificationMessage.PublishTime}";
        LoggerManager.Logger.Information($"Processing event : {eventDetails}");

        _sessionManager.TryGetSession(notificationEvent.Subscription.Session.SessionId.ToString(), out Entities.OPCUASession session);
        _subscriptionManager.GetSubscription(session.SessionNodeId, notificationEvent.Subscription.Id, out Opc.Application.Requests.SubscriptionRequest subscriptionGuid);

        SubscriptionValue subscriptionResponce = new()
        {
            DisplayName = notificationEvent.Subscription.DisplayName,
            OpcUaId = notificationEvent.Subscription.Id,
            ItemsCount = notificationEvent.Subscription.MonitoredItemCount,
            PublishingInterval = notificationEvent.Subscription.PublishingInterval,
            SequenceNumber = notificationEvent.Subscription.SequenceNumber,
            SessionNodeId = notificationEvent.Subscription.Session.SessionId.ToString(),
            PublishingEnabled = notificationEvent.Subscription.PublishingEnabled,
            Guid = subscriptionGuid.Guid,

        };

        foreach (MonitoredItemNotification change in changes)
        {
            MonitoredItem monitoringItem = notificationEvent.Subscription.FindItemByClientHandle(change.ClientHandle);
            if (monitoringItem == null)
            {
                continue;
            }

            MonitoredItemValue nodeValue = new()
            {
                SessionGuid = session.Guid,
                SubscriptionGuid = subscriptionGuid.Guid,
                SubscriptionOpcId = notificationEvent.Subscription.Id,
                DisplayName = monitoringItem.DisplayName,
                StartNodeId = monitoringItem.StartNodeId.ToString(),
                CreatedAt = change.Value.SourceTimestamp.ToLocalTime(),
                QueueSize = monitoringItem.QueueSize,
                Value = change.Value.WrappedValue.Value.ToString(),
                SamplingInterval = monitoringItem.SamplingInterval,
            };

            subscriptionResponce.MonitoredItems.Add(nodeValue);

            LoggerManager.Logger.Information($"[monitoring node] NodeId: {nodeValue.StartNodeId} sequence: {subscriptionResponce.SequenceNumber}");
        }

        return subscriptionResponce;
    }

}
