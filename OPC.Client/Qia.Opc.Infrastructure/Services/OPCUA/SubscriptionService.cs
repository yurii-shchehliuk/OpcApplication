using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Opc.Ua;
using Opc.Ua.Client;
using Qia.Opc.Domain.Core;
using Qia.Opc.Domain.Entities;
using Qia.Opc.Infrastructure.Application;
using Qia.Opc.OPCUA.Connector.Managers;
using Qia.Opc.Persistence.Repository;
using QIA.Opc.Domain.Request;
using QIA.Opc.Domain.Response;
using QIA.Opc.Infrastructure.Services.Communication;

namespace QIA.Opc.Infrastructure.Services.OPCUA
{
	public class SubscriptionService
	{
		private readonly SessionManager sessionManager;
		private readonly SubscriptionManager subscriptionManager;
		private readonly SignalRService signalRService;
		private readonly IMapper mapper;
		private readonly NodeService nodeService;

		public SubscriptionService(SessionManager sessionManager,
							 SubscriptionManager subscriptionManager,
							 SignalRService signalRService,
							 NodeService nodeService,
							 IMapper mapper)
		{
			this.sessionManager = sessionManager;
			this.subscriptionManager = subscriptionManager;
			this.signalRService = signalRService;
			this.mapper = mapper;
			this.nodeService = nodeService;
			subscriptionManager.Session_NotificationEvent += SubscriptionManager_SessionNotificationEvent;
		}

		public async Task SubscribeAsync(SubscriptionParameters subsParams, string nodeId)
		{
			try
			{
				var subscriptionId = subscriptionManager.Subscribe(subsParams, nodeId);

				//var newNodeRef = await nodeService.FindNodeByNodeIdAsync(nodeId);
				//newNodeRef.SubscriptionId = subscriptionId.ToString();
				//await nodeService.UpsertConfigAsync(newNodeRef);
			}
			catch (Exception ex)
			{
				LoggerManager.Logger.Error("Cannot subscribe to the node: {0} {1}", ex);
			}
			await Task.CompletedTask;
		}

		public async Task AddToSubscription(string subscriptionName, string nodeId)
		{
			try
			{
				subscriptionManager.AddToSubscription(subscriptionName, nodeId);
			}
			catch (Exception ex)
			{
				LoggerManager.Logger.Error("Cannot add to the subscription: {0} {1}", ex);
			}
			await Task.CompletedTask;
		}

		public async Task DeleteSubscriptionAsync(uint subscriptionId)
		{
			subscriptionManager.DeleteSubscription(subscriptionId);
			await Task.CompletedTask;
		}

		public async Task GetActiveSubscriptions()
		{
			var subscriptions = subscriptionManager.GetActiveSubscriptions();
			var subscriptionsDTO = mapper.Map<IEnumerable<Domain.Response.SubscriptionResponce>>(subscriptions);

			foreach (var item in subscriptionsDTO)
			{
				await signalRService.SendSubscriptionAsync(item, item.SessionNodeId);
			}
		}

		public void SetPublishingMode(string subscriptionId, bool enable)
		{
			subscriptionManager.SetPublishingMode(subscriptionId, enable);
		}

		public void ModifySubscription(SubscriptionParameters subsParams, uint subscriptionId)
		{
			this.subscriptionManager.Modify(subsParams, subscriptionId);
		}

		public void DeleteMonitoringItem(uint subscriptionId, string nodeId)
		{
			subscriptionManager.DeleteMonitoringItem
						(subscriptionId, nodeId);
		}

		private async Task SendSubscriptionAsync(SubscriptionResponce subscriptionResponce)
		{
			LoggerManager.Logger.Information($"[monitoring subscription] Name: {subscriptionResponce.DisplayName} SessionId: {subscriptionResponce.SessionNodeId} Interval:{subscriptionResponce.PublishInterval}");

			await signalRService.SendSubscriptionAsync(subscriptionResponce, subscriptionResponce.SessionNodeId);
		}

		/// <summary>
		/// avoid stackoverflow
		/// </summary>
		private bool isHandlingEvent = false;
		private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
		private async void SubscriptionManager_SessionNotificationEvent(object senderSession, NotificationEventArgs e)
		{
			if (isHandlingEvent)
			{
				isHandlingEvent = false;
				return;
			}
			isHandlingEvent = true;
			//await _semaphore.WaitAsync();

			try
			{
				// get the changes.
				List<MonitoredItemNotification> changes = new List<MonitoredItemNotification>();
				foreach (MonitoredItemNotification change in e.NotificationMessage.GetDataChanges(false))
				{
					if (e.Subscription.FindItemByClientHandle(change.ClientHandle) == null)
					{
						continue;
					}
					changes.Add(change);
				}

				if (changes.Count == 0 || e.Subscription.Session.SessionId == null) return;

				var subscriptionResponce = new SubscriptionResponce
				{
					DisplayName = e.Subscription.DisplayName,
					Id = e.Subscription.Id,
					ItemsCount = e.Subscription.MonitoredItemCount,
					PublishInterval = e.Subscription.PublishingInterval,
					SequenceNumber = e.Subscription.SequenceNumber,
					SessionNodeId = e.Subscription.Session.SessionId.ToString(),
					PublishingEnabled = e.Subscription.PublishingEnabled,
				};

				foreach (var change in changes)
				{
					var monitoringItem = e.Subscription.FindItemByClientHandle(change.ClientHandle);
					if (monitoringItem == null)
						continue;

					subscriptionResponce.MonitoringItems.Add(new MonitoredItemResponse()
					{
						DisplayName = monitoringItem.DisplayName,
						StartNodeId = monitoringItem.StartNodeId.ToString(),
						SourceTime = change.Value.SourceTimestamp.ToLocalTime(),
						QueueSize = monitoringItem.QueueSize,
						Value = change.Value.WrappedValue.Value,
						SamplingInterval = monitoringItem.SamplingInterval,
					});

					var nodeValue = new NodeValue
					{
						DisplayName = monitoringItem.DisplayName,
						NodeId = monitoringItem.StartNodeId.ToString(),
						StoreTime = change.Value.SourceTimestamp.ToLocalTime(),
						MSecs = subscriptionResponce.PublishInterval,
						Value = change.Value.WrappedValue.Value.ToString(),
						SessionName = subscriptionResponce.SessionNodeId
					};
					LoggerManager.Logger.Information($"[monitoring node] NodeId: {nodeValue.NodeId}");

					await signalRService.SendNodeAsync(nodeValue, subscriptionResponce.SessionNodeId);
					await nodeService.AddNodeValueAsync(nodeValue);
					//await _azureMS.SendNodeAsync(entity);
				}

				await SendSubscriptionAsync(subscriptionResponce);
			}
			catch (Exception ex)
			{
				LoggerManager.Logger.Error($"Collection error, {ex.Message}");
			}
			finally
			{
				//_semaphore.Release();
				isHandlingEvent = false;
			}
		}
	}
}