using Opc.Ua;
using Opc.Ua.Client;
using QIA.Plugin.OpcClient.Core;
using QIA.Plugin.OpcClient.DTOs;
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
        //private static HashSet<NodeConfig> itemsToFind = null;

        private readonly INodeManager _nm;
        private readonly IDataAccess _repo;
        private readonly IAzureMessageService _azureMS;

        // core
        private readonly Session m_session;
        private Subscription m_subscription;
        // events
        private NotificationEventHandler m_SessionNotification;
        private EventHandler m_PublishStatusChanged;
        private SubscriptionStateChangedEventHandler m_SubscriptionStateChanged;

        public SubscriptionManager(INodeManager nm, IDataAccess repo, IAzureMessageService azureMessageService)
        {
            _nm = nm;
            _repo = repo;
            _azureMS = azureMessageService;
            m_session = AppConfiguration.OpcUaClientSession;
            m_SessionNotification = new NotificationEventHandler(Session_NotificationAsync);
            try
            {
                //itemsToFind = JsonSerializer.Deserialize<HashSet<NodeConfig>>(File.ReadAllText(".\\nodemanager.json"));
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Couldn't deserialize nodemanager");
            }
        }

        public void SubscribeNew(ReferenceDescription reference)
        {
            try
            {
                if (reference == null)
                {
                    return;
                }

                if (m_session != null)
                {
                    Subscription newSubscription = new Subscription(m_session.DefaultSubscription);
                    m_session.AddSubscription(newSubscription);
                    newSubscription.Create();

                    Subscription duplicateSubscription = m_session.Subscriptions.FirstOrDefault(s => s.Id != 0 && s.Id.Equals(newSubscription.Id) && s != newSubscription);
                    if (duplicateSubscription != null)
                    {
                        Console.WriteLine("Duplicate subscription was created with the id: {0}", duplicateSubscription.Id);

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
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
                throw;
            }
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
        /// avoid stackoverflow
        /// </summary>
        private bool isHandlingEvent = false;
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        private async void Session_NotificationAsync(Session session, NotificationEventArgs e)
        {
            if (isHandlingEvent)
            {
                return;
            }

            isHandlingEvent = true;

            await _semaphore.WaitAsync();
            try
            {
                await HandleSessionItems();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
            finally
            {
                isHandlingEvent = false;
                _semaphore.Release();
                m_SessionNotification.Invoke(m_session, e);
            }
        }

        private async Task HandleSessionItems()
        {
            foreach (var subscriptionItems in m_session.Subscriptions)
            {
                foreach (var monitoringItem in subscriptionItems.MonitoredItems)
                {
                    string currentNodeId = monitoringItem.ResolvedNodeId.Identifier.ToString();
                    var nodeConfig = TreeManager.FindNode(currentNodeId);

                    if (nodeConfig != null)
                    {
                        var nodeValueNew = _nm.ReadNodeValue(monitoringItem.ResolvedNodeId);
                        var node = await _repo.FindByIdAsync(nodeConfig.NodeId);

                        if (node == null)
                        {
                            await AddNewNode(nodeConfig.NodeId, nodeValueNew);
                            return;
                        }

                        if (nodeValueNew == node.Value || nodeValueNew is null)
                            return;

                        await AddByMilliseconds(node, nodeValueNew, nodeConfig);

                        await AddByRange(node, nodeValueNew, nodeConfig);
                    }
                }
            }
        }

        private async Task AddNewNode(string currentNodeId, string nodeValueNew)
        {
            var nodeDto = TreeManager.FindNode(currentNodeId);
            SampleNode entity = new()
            {
                Value = nodeValueNew,
                NodeId = nodeDto.NodeId,
                Name = nodeDto.Name,
                MSecs = nodeDto.Msecs,
                Range = nodeDto.Range,
                NodeType = nodeDto.NodeType,
            };

            _repo.AddAsync(entity).Wait();

            await _azureMS.SendMessageAsync(entity);
        }

        private async Task UpdateNodeValue(SampleNode node, string nodeValueNew)
        {
            var nodeDto = TreeManager.FindNode(node.NodeId);
            node.MSecs = nodeDto.Msecs;
            node.Range = nodeDto.Range;
            node.NodeType = nodeDto.NodeType;
            node.Value = nodeValueNew;
            node.StoreTime = DateTime.UtcNow;
            await _repo.UpdateAsync(node);
        }

        private async Task AddByMilliseconds(SampleNode node, string nodeValueNew, NodeConfig nodeConfig)
        {
            if (nodeConfig.Msecs == 0)
                return;

            var diff = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - new DateTimeOffset(node.StoreTime).ToUnixTimeMilliseconds();

            if (nodeConfig.Msecs > 0 && diff >= nodeConfig.Msecs)
            {
                await AddNewNode(nodeConfig.NodeId, nodeValueNew);
                //await UpdateNodeValue(node, nodeValueNew);
                Logger.Information($"[monitoring msecs] {nodeConfig.Name} {nodeValueNew}");
            }
        }

        List<TempNode> tempNodes = new();
        private async Task AddByRange(SampleNode node, string nodeValueNew, NodeConfig nodeConfig)
        {
            if (nodeConfig.Range == 0)
                return;

            int tempDbCount = tempNodes.Where(c => c.NodeId == nodeConfig.NodeId).Count();

            // update node per range
            if (tempDbCount < nodeConfig.Range)
            {
                var nodeDto = TreeManager.FindNode(node.NodeId);

                tempNodes.Add(new()
                {
                    Value = nodeValueNew,
                    Name = nodeDto.Name,
                    NodeId = nodeConfig.NodeId,
                    MSecs = nodeDto.Msecs,
                    Range = nodeDto.Range,
                    NodeType = nodeDto.NodeType,
                });
            }
            else
            {
                //await UpdateNodeValue(node, nodeValueNew);
                await AddNewNode(nodeConfig.NodeId, nodeValueNew);
                tempNodes = new List<TempNode>();
                Logger.Information($"[monitoring range] {nodeConfig.Name} {nodeValueNew}");
            }
        }
    }
}
