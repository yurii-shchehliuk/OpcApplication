namespace QIA.Opc.Infrastructure.Managers;

using global::Opc.Ua;
using global::Opc.Ua.Client;
using QIA.Opc.Application.Requests;
using QIA.Opc.Infrastructure.Application;

public class MonitoredItemManager
{
    private readonly SessionManager _sessionManager;

    public MonitoredItemManager(SessionManager sessionManager)
    {
        _sessionManager = sessionManager;
    }

    public MonitoredItem AddToSubscription(string sessionGuid, uint subscriptionId, string nodeId)
    {
        _sessionManager.TryGetSession(sessionGuid, out Entities.OPCUASession session);

        Subscription subscription = session.Session.Subscriptions.FirstOrDefault(s => s.Id == subscriptionId);
        if (subscription == null)
        {
            return null;
        }

        return AddMonitoredItem(session.Session, subscription, nodeId);
    }

    public MonitoredItem AddToSubscription(string sessionGuid, Subscription subscription, string nodeId)
    {
        _sessionManager.TryGetSession(sessionGuid, out Entities.OPCUASession session);

        return AddMonitoredItem(session.Session, subscription, nodeId);
    }

    public MonitoredItem GetMonitoringItem(string sessionNodeId, uint subscriptionId, string nodeId)
    {
        _sessionManager.TryGetSession(sessionNodeId, out Entities.OPCUASession session);
        Subscription subscription = session.Session.Subscriptions.FirstOrDefault(s => s.Id == subscriptionId);
        if (subscription == null)
        {
            return null;
        }

        MonitoredItem monItem = subscription.MonitoredItems.FirstOrDefault(c => c.StartNodeId.ToString() == nodeId);

        return monItem;
    }

    public MonitoredItem DeleteMonitoringItem(string sessionGuid, uint subscriptionId, string nodeId)
    {
        _sessionManager.TryGetSession(sessionGuid, out Entities.OPCUASession session);
        Subscription subscription = session.Session.Subscriptions.FirstOrDefault(s => s.Id == subscriptionId);
        if (subscription == null)
        {
            return null;
        }

        MonitoredItem monItem = subscription.MonitoredItems.FirstOrDefault(c => c.StartNodeId.ToString() == nodeId);
        subscription.RemoveItem(monItem);
        subscription.ApplyChanges();
        return monItem;
    }

    public bool UpdateMonitoredItem(string sessionGuid, MonitoredItemRequest updatedItem, uint subscriptionId)
    {
        _sessionManager.TryGetSession(sessionGuid, out Entities.OPCUASession session);

        Subscription subscription = session.Session.Subscriptions.FirstOrDefault(s => s.Id == subscriptionId);
        if (subscription == null)
        {
            return false;
        }

        MonitoredItem monItem = subscription.MonitoredItems.FirstOrDefault(c => c.StartNodeId.ToString() == updatedItem.StartNodeId);

        if (monItem == null)
        {
            return false;
        }

        monItem.DisplayName = updatedItem.DisplayName;
        monItem.SamplingInterval = updatedItem.SamplingInterval;
        monItem.QueueSize = updatedItem.QueueSize;
        monItem.DiscardOldest = updatedItem.DiscardOldest;

        subscription.ApplyChanges();

        return true;
    }

    private static MonitoredItem AddMonitoredItem(Session session, Subscription subscription, string nodeId)
    {
        try
        {
            NodeId opcNodeId = new(nodeId);
            BrowseDescription desc = new()
            {
                NodeId = opcNodeId,
                BrowseDirection = BrowseDirection.Forward,
                IncludeSubtypes = true,
                NodeClassMask = (uint)NodeClass.Variable | (uint)NodeClass.Object,
                ResultMask = (uint)BrowseResultMask.All
            };

            ResponseHeader nodeReferenceDescription = session.Browse(null, null, 0u, new BrowseDescriptionCollection() { desc }, out BrowseResultCollection results, out DiagnosticInfoCollection diagnosticInfos);

            if (nodeReferenceDescription == null)
            {
                LoggerManager.Logger.Error($"Node with ID {nodeId} not found.");
                return null;
            }
            Node nodeData = FindNodeOnServer(session, nodeId);

            MonitoredItem monitoredItem = new(subscription.DefaultItem)
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

            if (monitoredItem.Filter is EventFilter filter)
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

    private static Node FindNodeOnServer(Session session, string nodeId)
    {

        NodeId nodeIdToSearch = new(nodeId);

        Node node = session.ReadNode(nodeIdToSearch);

        return node;
    }
}
