using AutoMapper;
using Opc.Ua;
using Opc.Ua.Client;
using Qia.Opc.Domain.Common;
using Qia.Opc.Domain.Core;
using Qia.Opc.Domain.Entities;
using Qia.Opc.OPCUA.Connector.Managers;
using QIA.Opc.Domain.Entities;
using QIA.Opc.Domain.Repository;
using QIA.Opc.Domain.Requests;
using QIA.Opc.Domain.Responses;
using QIA.Opc.Infrastructure.Services.Communication;
using System.Threading;

namespace QIA.Opc.Infrastructure.Services.OPCUA
{
	/// <summary>
	/// </summary>
	/// <remarks>in this service every ApiResponse is bool because everything is based on events of the OPC UA library</remarks>
	public class SubscriptionService
	{
		private readonly SubscriptionManager subscriptionManager;
		private readonly IGenericRepository<SubscriptionConfig> subscriptionConfigRepo;
		private readonly SignalRService signalRService;
		private readonly IMapper mapper;
		private readonly MonitoredItemService monitoredItemService;

		public SubscriptionService(SubscriptionManager subscriptionManager,
							 SignalRService signalRService,
							 MonitoredItemService monitoredItemService,
							 IGenericRepository<SubscriptionConfig> subscriptionConfigRepo,
							 IMapper mapper)
		{
			this.subscriptionManager = subscriptionManager;
			this.signalRService = signalRService;
			this.monitoredItemService = monitoredItemService;
			this.subscriptionConfigRepo = subscriptionConfigRepo;
			this.mapper = mapper;
			subscriptionManager.Session_NotificationEvent += SubscriptionManager_SessionNotificationEvent;
		}

		public async Task<ApiResponse<SubscriptionValue>> SubscribeAsync(string sessionNodeId, SubscriptionRequest subsParams, string nodeId)
		{
			try
			{
				if (!nodeId.TryParseNodeId(out var node))
					return ApiResponse<SubscriptionValue>.Failure(HttpStatusCode.BadRequest, $"NodeId {nodeId} cannot be parsed");

				// create subscription
				var subscription = subscriptionManager.Subscribe(sessionNodeId, subsParams, out var session);
				if (subscription == null)
					return ApiResponse<SubscriptionValue>.Failure(HttpStatusCode.BadRequest);


				// prepare subscription
				var subscriptionGuidId = Guid.NewGuid().ToString();
				var subsConfig = mapper.Map<SubscriptionConfig>(subscription);
				subsConfig.SessionGuidId = session.SessionGuidId;
				subsConfig.SubscriptionGuidId = subscriptionGuidId;

				// add item to subscription
				var monItemResponse = monitoredItemService.AddItemToSubscription(sessionNodeId, subscription, nodeId);
				if (!monItemResponse.IsSuccess)
					return ApiResponse<SubscriptionValue>.Failure(HttpStatusCode.BadRequest, $"Cannot add to monitoring");

				//prepare and save
				var monItemCfg = mapper.Map<MonitoredItemConfig>(monItemResponse.Value);
				monItemCfg.SubscriptionGuidId = subscriptionGuidId;
				monItemCfg.GuidId = Guid.NewGuid().ToString();

				subsConfig.MonitoredItemsConfig.Add(monItemCfg);

				await subscriptionConfigRepo.AddAsync(subsConfig);

				// return updated subscription
				var subscriptionValue = mapper.Map<SubscriptionValue>(subscription);
				subscriptionValue.SubscriptionGuidId = subscriptionGuidId;

				return ApiResponse<SubscriptionValue>.Success(subscriptionValue);
			}
			catch (Exception ex)
			{
				return ApiResponse<SubscriptionValue>.Failure(HttpStatusCode.BadRequest, $"Cannot subscribe to the node: {ex.Message} \n{ex.InnerException?.Message ?? ""}");
			}
		}

		public async Task<ApiResponse<bool>> DeleteSubscriptionAsync(string sessionNodeId, uint subscriptionId, string subscriptionGuidId)
		{
			var result = subscriptionManager.DeleteSubscription(sessionNodeId, subscriptionId, out var subscriptionGuidIdMemo);

			var subsToDelete = await subscriptionConfigRepo.FindAsync(c => c.SubscriptionGuidId == (string.IsNullOrEmpty(subscriptionGuidIdMemo) ? subscriptionGuidId : subscriptionGuidIdMemo));
			await subscriptionConfigRepo.DeleteAsync(subsToDelete);

			return ApiResponse<bool>.Success(true);
		}

		public async Task<ApiResponse<bool>> ModifySubscriptionAsync(string sessionNodeId, SubscriptionRequest subsParams)
		{
			var result = this.subscriptionManager.Modify(sessionNodeId, subsParams, out var subscriptionGuidId);

			if (!result) return ApiResponse<bool>.Failure(HttpStatusCode.NotFound);

			var subsUpdated = await subscriptionConfigRepo.FindAsync(c => c.SubscriptionGuidId == subscriptionGuidId);

			mapper.Map(subsParams, subsUpdated);
			subsUpdated.UpdatedAt = DateTime.UtcNow;

			await subscriptionConfigRepo.UpdateAsync(subsUpdated);

			return ApiResponse<bool>.Success(true);
		}

		public ApiResponse<bool> SetPublishingMode(string sessionNodeId, uint subscriptionId, bool enable)
		{
			var result = subscriptionManager.SetPublishingMode(sessionNodeId, subscriptionId, enable);
			if (!result) return ApiResponse<bool>.HandledFailure(HttpStatusCode.NotFound, "Will recreate from db. There is not active subscription with such id.");

			return ApiResponse<bool>.Success(true);
		}

		public async Task<ApiResponse<bool>> GetSubscriptions(string sessionNodeId)
		{
			var activeSubscriptionsResponse = subscriptionManager.GetActiveOpcSubscriptions(sessionNodeId, out var sessionGuidId);
			var savedSubscriptionsResponse = await subscriptionConfigRepo.ListAllAsync(c => c.SessionGuidId == sessionGuidId, true, c => c.MonitoredItemsConfig);

			var activeSubscriptions = mapper.Map<IEnumerable<SubscriptionValue>>(activeSubscriptionsResponse);
			var savedSubscriptions = mapper.Map<IEnumerable<SubscriptionValue>>(savedSubscriptionsResponse);

			// Merging the subscriptions
			var mergedSubscriptions = savedSubscriptions
				.GroupJoin(activeSubscriptions,
					saved => saved.SubscriptionGuidId,
					active => active.SubscriptionGuidId,
					(saved, activeGroup) => activeGroup.Any() ? activeGroup.First() : saved)
				 .Select(subscription =>
				 {
					 subscription.SessionNodeId = sessionNodeId;
					 return subscription;
				 });

			foreach (var item in mergedSubscriptions)
			{
				await signalRService.SendSubscriptionAsync(item, sessionNodeId);
			}

			return ApiResponse<bool>.Success(true);
		}
		/// <param name="subscriptionId">when subscription is from value with opc</param>
		/// <param name="subscriptionGuidId">when subscription is from database</param>
		/// currently works only when subscribed. TODO: pass guid from UI
		public async Task<ApiResponse<SubscriptionConfig>> GetSubscriptionConfig(string sessionNodeId, uint subscriptionId, string subscriptionGuidId = "")
		{
			subscriptionManager.GetSubscription(sessionNodeId, subscriptionId, out var subscriptionGuidIdMemo);
			var subscription = await subscriptionConfigRepo.FindAsync(c => c.SubscriptionGuidId == (string.IsNullOrEmpty(subscriptionGuidIdMemo) ? subscriptionGuidId : subscriptionGuidIdMemo), true);
			if (subscription == null)
			{
				return ApiResponse<SubscriptionConfig>.Failure(HttpStatusCode.NotFound);
			}

			//handle unexpected
			return ApiResponse<SubscriptionConfig>.Success(subscription);
		}

		public async Task<ApiResponse<bool>> SubscribeFromModel(string sessionNodeId, SubscriptionRequest request)
		{
			// create opcSubscription
			var opcSubscription = subscriptionManager.Subscribe(sessionNodeId, request, out _);
			if (opcSubscription == null)
				return ApiResponse<bool>.Failure(HttpStatusCode.BadRequest);

			var subscription = await subscriptionConfigRepo.FindAsync(c => c.SubscriptionGuidId == request.SubscriptionGuidId,true, includes: c => c.MonitoredItemsConfig);

			// add items to subscription
			foreach (var monItemCfg in subscription.MonitoredItemsConfig)
			{
				var monItemResponse = monitoredItemService.AddItemToSubscription(sessionNodeId, opcSubscription, monItemCfg.StartNodeId);

				if (!monItemResponse.IsSuccess)
					return ApiResponse<bool>.Failure(HttpStatusCode.BadRequest, $"Cannot add to monitoring");
			}

			return ApiResponse<bool>.Success();
		}

		private async Task SendSubscriptionAsync(SubscriptionValue subscriptionResponce)
		{
			LoggerManager.Logger.Information($"[monitoring subscription] Name: {subscriptionResponce.DisplayName} SessionId: {subscriptionResponce.SessionNodeId} Interval:{subscriptionResponce.PublishingInterval}");

			await signalRService.SendSubscriptionAsync(subscriptionResponce, subscriptionResponce.SessionNodeId);
		}

		/// avoid stackoverflow
		private bool isHandlingEvent = false;
		private async void SubscriptionManager_SessionNotificationEvent(object senderSession, NotificationEventArgs e)
		{
			if (isHandlingEvent)
			{
				isHandlingEvent = false;
				return;
			}
			isHandlingEvent = true;
			//await _semaphore.WaitAsync();
			//#if DEBUG
			//			LoggerManager.Logger.Information($"Event recieved");
			//#endif
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

				var subscriptionResponce = new SubscriptionValue
				{
					DisplayName = e.Subscription.DisplayName,
					OpcUaId = e.Subscription.Id,
					ItemsCount = e.Subscription.MonitoredItemCount,
					PublishingInterval = e.Subscription.PublishingInterval,
					SequenceNumber = e.Subscription.SequenceNumber,
					SessionNodeId = e.Subscription.Session.SessionId.ToString(),
					PublishingEnabled = e.Subscription.PublishingEnabled,
				};

				foreach (var change in changes)
				{
					var monitoringItem = e.Subscription.FindItemByClientHandle(change.ClientHandle);
					if (monitoringItem == null)
						continue;

					var nodeValue = new MonitoredItemValue()
					{
						SubscriptionOpcId = e.Subscription.Id,
						DisplayName = monitoringItem.DisplayName,
						StartNodeId = monitoringItem.StartNodeId.ToString(),
						CreatedAt = change.Value.SourceTimestamp.ToLocalTime(),
						QueueSize = monitoringItem.QueueSize,
						Value = change.Value.WrappedValue.Value.ToString(),
						SamplingInterval = monitoringItem.SamplingInterval,
					};

					subscriptionResponce.MonitoredItems.Add(nodeValue);

					LoggerManager.Logger.Information($"[monitoring node] NodeId: {nodeValue.StartNodeId}");

					await signalRService.SendNodeAsync(nodeValue, subscriptionResponce.SessionNodeId);
					await monitoredItemService.SaveMonitoredItemValueAsync(nodeValue);
					//await _azureMS.SendNodeAsync(nodeValue);
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