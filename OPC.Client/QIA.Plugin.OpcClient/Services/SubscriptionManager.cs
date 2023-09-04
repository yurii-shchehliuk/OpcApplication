using Opc.Ua;
using Opc.Ua.Client;
using QIA.Plugin.OpcClient.Core;
using QIA.Plugin.OpcClient.Core.Settings;
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
        private readonly MessageService messageService;

        // core
        private Session m_session;
        private Subscription m_subscription;
        // events
        private NotificationEventHandler m_SessionNotification = null;
        private EventHandler m_PublishStatusChanged = null;
        private SubscriptionStateChangedEventHandler m_SubscriptionStateChanged = null;

        public SubscriptionManager(INodeManager nm, IDataAccess repo, IAzureMessageService azureMessageService, MessageService messageService)
        {
            _nm = nm;
            _repo = repo;
            _azureMS = azureMessageService;
            this.messageService = messageService;
            m_session = OpcConfiguration.OpcUaClientSession;
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
            if (m_session == null)
            {
                //TODO
                m_session = OpcConfiguration.OpcUaClientSession;

            }

            if (reference == null || m_session == null)
                return;

            try
            {
                Subscription newSubscription = new Subscription(m_session.DefaultSubscription);
                m_session.AddSubscription(newSubscription);
                newSubscription.Create();

                Subscription duplicateSubscription = m_session.Subscriptions.FirstOrDefault(s => s.Id != 0 && s.Id.Equals(newSubscription.Id) && s != newSubscription);
                if (duplicateSubscription != null)
                {
                    LoggerManager.Logger.Information("Duplicate subscription was created with the id: {0}", duplicateSubscription.Id);

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
                    LoggerManager.Logger.Information("Item added to monitoring");
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
                throw;
            }
        }

        public void SubscribeNew(string nodeId)
        {
            var node = _nm.GetNodeIdFromId(nodeId);
            _nm.ReadNodeValue(node);
            _nm.ReadNode(node);
            _nm.FindInCache(nodeId);
            SubscribeNew(reference: null);

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
        /// avoid stackoverflow, skip first event
        /// </summary>
        private bool isHandlingEvent = true;
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        private async void Session_NotificationAsync(Session session, NotificationEventArgs e)
        {
            //TODO: configure server events listening.
            if (isHandlingEvent)
            {
                isHandlingEvent = false;
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
                Thread.Sleep(1200);
                _semaphore.Release();
                m_SessionNotification.Invoke(m_session, e);
                isHandlingEvent = false;
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

                        if (node == null && nodeValueNew != null)
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

        private async Task UpdateNodeValue(NodeData node, string nodeValueNew)
        {
            var nodeDto = TreeManager.FindNode(node.NodeId);
            node.MSecs = nodeDto.Msecs;
            node.Range = nodeDto.Range;
            node.NodeType = nodeDto.NodeType;
            node.Value = nodeValueNew;
            node.StoreTime = DateTime.UtcNow;
            await _repo.UpdateAsync(node);
        }

        private async Task AddByMilliseconds(NodeData node, string nodeValueNew, NodeConfig nodeConfig)
        {
            if (nodeConfig.Msecs == 0)
                return;

            var diff = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - new DateTimeOffset(node.StoreTime).ToUnixTimeMilliseconds();

            if (nodeConfig.Msecs > 0 && diff >= nodeConfig.Msecs)
            {
                Logger.Information($"[monitoring msecs] {nodeConfig.Name} {nodeValueNew} {DateTimeOffset.UtcNow}");
                await AddNewNode(nodeConfig.NodeId, nodeValueNew);
                //await UpdateNodeValue(node, nodeValueNew);
            }
        }

        List<NodeData> tempNodes = new();
        private async Task AddByRange(NodeData node, string nodeValueNew, NodeConfig nodeConfig)
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
                    StoreTime = DateTime.UtcNow
                });
            }
            else
            {
                //await UpdateNodeValue(node, nodeValueNew);
                Logger.Information($"[monitoring range] {nodeConfig.Name} {nodeValueNew} {nodeValueNew} {DateTimeOffset.UtcNow}");
                await AddNewNode(nodeConfig.NodeId, nodeValueNew);
                tempNodes.RemoveAll(c => c.NodeId == nodeConfig.NodeId);
            }
        }

        private async Task AddNewNode(string currentNodeId, string nodeValueNew)
        {
            var nodeDto = TreeManager.FindNode(currentNodeId);
            NodeData entity = new()
            {
                Value = nodeValueNew,
                NodeId = nodeDto.NodeId,
                Name = nodeDto.Name,
                MSecs = nodeDto.Msecs,
                Range = nodeDto.Range,
                NodeType = nodeDto.NodeType,
                StoreTime = DateTime.UtcNow
            };

            if (Extensions.ReadSettings().SaveToDb)
            {
                _repo.AddAsync(entity).Wait();
            }

            await messageService.SendMessage(entity);
            await _azureMS.SendMessageAsync(entity);
        }
    }
}
