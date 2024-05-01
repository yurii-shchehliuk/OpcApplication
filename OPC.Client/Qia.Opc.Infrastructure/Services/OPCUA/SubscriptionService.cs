namespace QIA.Opc.Infrastructure.Services.OPCUA;

using System.Net;
using AutoMapper;
using global::Opc.Ua.Client;
using Qia.Opc.Domain.Entities;
using QIA.Opc.Application.Requests;
using QIA.Opc.Application.Responses;
using QIA.Opc.Domain.Entities;
using QIA.Opc.Domain.Repositories;
using QIA.Opc.Infrastructure.Application;
using QIA.Opc.Infrastructure.Extensions;
using QIA.Opc.Infrastructure.Managers;
using QIA.Opc.Infrastructure.Services.Communication;

/// <summary>
/// </summary>
/// <remarks>in this service every ApiResponse is bool because everything is based on events of the OPC UA library</remarks>
public class SubscriptionService
{
    private readonly SubscriptionManager _subscriptionManager;
    private readonly IGenericRepository<SubscriptionConfig> _subscriptionConfigRepo;
    private readonly SignalRService _signalRService;
    private readonly IMapper _mapper;
    private readonly QueueService _queueService;
    private readonly MonitoredItemService _monitoredItemService;
    private readonly AzureMessageService _azureMS;

    public SubscriptionService(SubscriptionManager subscriptionManager,
                         SignalRService signalRService,
                         MonitoredItemService monitoredItemService,
                         AzureMessageService azureMS,
                         IGenericRepository<SubscriptionConfig> subscriptionConfigRepo,
                         QueueService queueService,
                         IMapper mapper)
    {
        _subscriptionManager = subscriptionManager;
        _signalRService = signalRService;
        _monitoredItemService = monitoredItemService;
        _azureMS = azureMS;
        _subscriptionConfigRepo = subscriptionConfigRepo;
        _mapper = mapper;
        _queueService = queueService;
        _subscriptionManager.Session_NotificationEvent += SubscriptionManager_SessionNotificationEvent;
    }

    public async Task<ApiResponse<SubscriptionValue>> SubscribeAsync(string sessionNodeId, SubscriptionRequest subsParams, string nodeId)
    {
        try
        {
            if (!nodeId.TryParseNodeId(out global::Opc.Ua.NodeId node))
            {
                return ApiResponse<SubscriptionValue>.Failure(HttpStatusCode.BadRequest, $"NodeId {nodeId} cannot be parsed");
            }

            if (subsParams.Guid == "")
            {
                subsParams.Guid = Guid.NewGuid().ToString();
            }

            // create subscription
            Subscription subscription = _subscriptionManager.Subscribe(sessionNodeId, subsParams, out Entities.OPCUASession session);
            if (subscription == null)
            {
                return ApiResponse<SubscriptionValue>.Failure(HttpStatusCode.BadRequest);
            }

            // prepare subscription config
            SubscriptionConfig subsConfig = _mapper.Map<SubscriptionConfig>(subscription);
            subsConfig.SessionGuid = session.Guid;
            subsConfig.Guid = subsParams.Guid;
            subsConfig.MaxValue = subsParams.MaxValue;
            subsConfig.MinValue = subsParams.MinValue;
            // add item to subscription
            ApiResponse<MonitoredItem> monItemResponse = _monitoredItemService.AddItemToSubscription(sessionNodeId, subscription, nodeId);
            if (!monItemResponse.IsSuccess)
            {
                return ApiResponse<SubscriptionValue>.Failure(HttpStatusCode.BadRequest, $"Cannot add {nodeId} to monitoring");
            }

            //prepare and save
            MonitoredItemConfig monItemCfg = _mapper.Map<MonitoredItemConfig>(monItemResponse.Value);
            monItemCfg.SubscriptionGuid = subsConfig.Guid;

            subsConfig.MonitoredItemsConfig.Add(monItemCfg);

            await _subscriptionConfigRepo.AddAsync(subsConfig);

            // return updated subscription
            SubscriptionValue subscriptionValue = _mapper.Map<SubscriptionValue>(subscription);
            subscriptionValue.Guid = subsConfig.Guid;
            subscriptionValue.MaxValue = subsParams.MaxValue;
            subscriptionValue.MinValue = subsParams.MinValue;

            return ApiResponse<SubscriptionValue>.Success(subscriptionValue);
        }
        catch (Exception ex)
        {
            return ApiResponse<SubscriptionValue>.Failure(HttpStatusCode.BadRequest, $"Cannot subscribe to the node: {ex.Message} \n{ex.InnerException?.Message ?? ""}");
        }
    }

    public async Task<ApiResponse<SubscriptionValue>> SubscribeFromDbModel(string sessionNodeId, SubscriptionRequest request)
    {
        // create opcSubscription
        Subscription opcSubscription = _subscriptionManager.Subscribe(sessionNodeId, request, out _);
        if (opcSubscription == null)
        {
            return ApiResponse<SubscriptionValue>.Failure(HttpStatusCode.BadRequest);
        }

        SubscriptionConfig subscription = await _subscriptionConfigRepo.FindAsync(c => c.Guid == request.Guid, true, includes: c => c.MonitoredItemsConfig);

        // add items to subscription
        foreach (MonitoredItemConfig monItemCfg in subscription.MonitoredItemsConfig)
        {
            ApiResponse<MonitoredItem> monItemResponse = _monitoredItemService.AddItemToSubscription(sessionNodeId, opcSubscription, monItemCfg.StartNodeId);

            if (!monItemResponse.IsSuccess)
            {
                return ApiResponse<SubscriptionValue>.Failure(HttpStatusCode.BadRequest, $"Cannot add to monitoring");
            }
        }

        SubscriptionValue subscriptionValue = _mapper.Map<SubscriptionValue>(opcSubscription);
        subscriptionValue.MaxValue = subscription.MaxValue;
        subscriptionValue.MinValue = subscription.MinValue;
        return ApiResponse<SubscriptionValue>.Success(subscriptionValue);
    }

    public async Task<ApiResponse<IEnumerable<SubscriptionValue>>> GetSubscriptions(string sessionNodeId)
    {
        IEnumerable<Entities.OPCUASubscription> activeSubscriptionsResponse = _subscriptionManager.GetActiveOpcSubscriptions(sessionNodeId, out var sessionGuid);
        IEnumerable<SubscriptionConfig> savedSubscriptionsResponse = await _subscriptionConfigRepo.ListAllAsync(c => c.SessionGuid == sessionGuid, true, c => c.MonitoredItemsConfig);

        IEnumerable<SubscriptionValue> activeSubscriptions = _mapper.Map<IEnumerable<SubscriptionValue>>(activeSubscriptionsResponse);
        IEnumerable<SubscriptionValue> savedSubscriptions = _mapper.Map<IEnumerable<SubscriptionValue>>(savedSubscriptionsResponse);

        // Merging the subscriptions
        IEnumerable<SubscriptionValue> mergedSubscriptions = savedSubscriptions
            .GroupJoin(activeSubscriptions,
                saved => saved.Guid,
                active => active.Guid,
                (saved, activeGroup) => activeGroup.Any() ? activeGroup.First() : saved)
             .Select(subscription =>
             {
                 subscription.SessionNodeId = sessionNodeId;
                 return subscription;
             });

        try
        {
            //TODO: get rid of that
            foreach (SubscriptionValue item in mergedSubscriptions)
            {
                await _signalRService.SendSubscriptionAsync(item, sessionNodeId);
            }
        }
        catch (Exception ex)
        {
            LoggerManager.Logger.Error("Get subscriptions: {0}", ex.Message);
        }

        return ApiResponse<IEnumerable<SubscriptionValue>>.Success(mergedSubscriptions);
    }

    /// <param name="subscriptionId">when subscription is from value with opc server</param>
    /// <param name="subscriptionGuid">when subscription is from database</param>
    /// currently works only when subscribed. TODO: pass guid from UI
    public async Task<ApiResponse<SubscriptionConfig>> GetSubscriptionConfig(string subscriptionGuid)
    {
        SubscriptionConfig subscription = await _subscriptionConfigRepo.FindAsync(c => c.Guid == subscriptionGuid, true);
        if (subscription == null)
        {
            return ApiResponse<SubscriptionConfig>.Failure(HttpStatusCode.NotFound);
        }

        //handle unexpected
        return ApiResponse<SubscriptionConfig>.Success(subscription);
    }

    public async Task<ApiResponse<SubscriptionConfig>> ModifySubscriptionAsync(string sessionNodeId, SubscriptionRequest subsParams)
    {
        _subscriptionManager.Modify(sessionNodeId, subsParams);

        SubscriptionConfig subsUpdated = await _subscriptionConfigRepo.FindAsync(c => c.Guid == subsParams.Guid);

        _mapper.Map(subsParams, subsUpdated);
        subsUpdated.UpdatedAt = DateTime.UtcNow;

        await _subscriptionConfigRepo.UpdateAsync(subsUpdated);

        return ApiResponse<SubscriptionConfig>.Success(subsUpdated);
    }

    public ApiResponse<SubscriptionValue> SetPublishingMode(string sessionNodeId, uint subscriptionId, SubscriptionRequest request)
    {
        Subscription result = _subscriptionManager.SetPublishingMode(sessionNodeId, subscriptionId, request.PublishingEnabled);

        if (result == null)
        {
            return ApiResponse<SubscriptionValue>.Success(null, HttpStatusCode.NotFound);
        }

        SubscriptionValue subscriptionValue = _mapper.Map<SubscriptionValue>(result);
        subscriptionValue.MaxValue = request.MaxValue;
        subscriptionValue.MinValue = request.MinValue;
        return ApiResponse<SubscriptionValue>.Success(subscriptionValue);
    }

    public ApiResponse<bool> StopAllSubscriptions(string sessionNodeId)
    {
        IEnumerable<Entities.OPCUASubscription> activeSubscriptionsResponse = _subscriptionManager.GetActiveOpcSubscriptions(sessionNodeId, out _);

        foreach (Entities.OPCUASubscription item in activeSubscriptionsResponse)
        {
            _subscriptionManager.SetPublishingMode(sessionNodeId, item.Subscription.Id, false);
        }

        return ApiResponse<bool>.Success();
    }

    public async Task<ApiResponse<bool>> DeleteSubscriptionAsync(string sessionNodeId, uint subscriptionId, string subscriptionGuid)
    {
        _subscriptionManager.DeleteSubscription(sessionNodeId, subscriptionId, out _);

        SubscriptionConfig subsToDelete = await _subscriptionConfigRepo.FindAsync(c => c.Guid == subscriptionGuid);
        await _subscriptionConfigRepo.DeleteAsync(subsToDelete);

        return ApiResponse<bool>.Success(true);
    }

    #region events


    private void SubscriptionManager_SessionNotificationEvent(object senderSession, NotificationEventArgs e)
    {
        try
        {
            List<global::Opc.Ua.MonitoredItemNotification> changes = _queueService.GetChanges(e);
            if (changes == null)
            {
                return;
            }

            SubscriptionValue subscriptionValue = _queueService.GetSubscription(changes);

            if (subscriptionValue == null)
            {
                return;
            }
            //TODO: check if subscription is unique
            SendSubscription(subscriptionValue);
        }
        catch (Exception ex)
        {
            LoggerManager.Logger.Error($"New changes processing: {ex.Message}");
        }
        finally
        {
            QueueService.IsHandlingEvent = false;
        }
    }

    private void SendSubscription(SubscriptionValue subscriptionRequest) =>
        Task.Run(async () =>
            {
                LoggerManager.Logger.Information($"[monitoring subscription] Name: {subscriptionRequest.DisplayName} SessionId: {subscriptionRequest.SessionNodeId} Interval:{subscriptionRequest.PublishingInterval}");

                await _signalRService.SendSubscriptionAsync(subscriptionRequest, subscriptionRequest.SessionNodeId);

                foreach (MonitoredItemValue monitoredItem in subscriptionRequest.MonitoredItems)
                {
                    if (!float.TryParse(monitoredItem.Value, out var itemValue))
                    {
                        continue;
                    }

                    if (itemValue > subscriptionRequest.MaxValue || itemValue < subscriptionRequest.MinValue)
                    {
                        LoggerManager.Logger.Warning($"[node beyond the range] NodeId: {monitoredItem.StartNodeId} - {monitoredItem.Value}. Max: {subscriptionRequest.MaxValue} Min: {subscriptionRequest.MinValue}");
                        await _azureMS.SendNodeAsync(monitoredItem);
                    }

                    await _signalRService.SendNodeAsync(monitoredItem, subscriptionRequest.SessionNodeId); //TODO: eliminate it
                    await _monitoredItemService.SaveMonitoredItemValueAsync(monitoredItem);
                }
            });
    #endregion
}
