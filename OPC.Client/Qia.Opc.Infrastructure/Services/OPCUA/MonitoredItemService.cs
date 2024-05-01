namespace QIA.Opc.Infrastructure.Services.OPCUA;

using System.Linq.Expressions;
using System.Net;
using AutoMapper;
using global::Opc.Ua.Client;
using Qia.Opc.Domain.Entities;
using QIA.Opc.Application.Requests;
using QIA.Opc.Domain.Entities;
using QIA.Opc.Domain.Repositories;
using QIA.Opc.Infrastructure.Application;
using QIA.Opc.Infrastructure.Extensions;
using QIA.Opc.Infrastructure.Managers;

public class MonitoredItemService
{
    private readonly MonitoredItemManager _monitoredItemManager;
    private readonly IMapper _mapper;
    private readonly IGenericRepository<MonitoredItemConfig> _monItemCfgRepo;
    private readonly IGenericRepository<MonitoredItemValue> _monitoredItemValueRepo;

    public MonitoredItemService(MonitoredItemManager monitoredItemManager,
        IMapper mapper,
        IGenericRepository<MonitoredItemConfig> monitoredItemConfigRepo,
        IGenericRepository<MonitoredItemValue> monitoredItemValueRepo)
    {
        _monitoredItemManager = monitoredItemManager;
        _mapper = mapper;
        _monItemCfgRepo = monitoredItemConfigRepo;
        _monitoredItemValueRepo = monitoredItemValueRepo;
    }

    public ApiResponse<MonitoredItem> AddItemToSubscription(string sessionNodeId, uint subscriptionId, string nodeId)
    {
        try
        {
            if (!nodeId.TryParseNodeId(out global::Opc.Ua.NodeId node))
            {
                return ApiResponse<MonitoredItem>.Failure(HttpStatusCode.BadRequest, $"NodeId {nodeId} cannot be parsed");
            }

            MonitoredItem item = _monitoredItemManager.AddToSubscription(sessionNodeId, subscriptionId, nodeId);
            return ApiResponse<MonitoredItem>.Success(item);

        }
        catch (Exception ex)
        {
            return ApiResponse<MonitoredItem>.Failure(HttpStatusCode.BadRequest, $"Cannot add to the subscription:: {ex.Message} \n{ex.InnerException?.Message ?? ""}");
        }
    }

    public ApiResponse<MonitoredItem> AddItemToSubscription(string sessionNodeId, Subscription subscription, string nodeId)
    {
        try
        {
            if (!nodeId.TryParseNodeId(out global::Opc.Ua.NodeId node))
            {
                return ApiResponse<MonitoredItem>.Failure(HttpStatusCode.BadRequest, $"NodeId {nodeId} cannot be parsed");
            }

            MonitoredItem item = _monitoredItemManager.AddToSubscription(sessionNodeId, subscription, nodeId);
            return ApiResponse<MonitoredItem>.Success(item);

        }
        catch (Exception ex)
        {
            return ApiResponse<MonitoredItem>.Failure(HttpStatusCode.BadRequest, $"Cannot add {nodeId} to the subscription:: {ex.Message} \n{ex.InnerException?.Message ?? ""}");
        }
    }

    public async Task SaveMonitoredItemValueAsync(MonitoredItemValue monitoredItemValue)
    {
        try
        {
            await _monitoredItemValueRepo.AddAsync(monitoredItemValue);
        }
        catch (Exception ex)
        {
            LoggerManager.Logger.Error("Couldnt store item value to db: {0}", ex);
        }
    }

    public ApiResponse<MonitoredItem> GetMonitoringItem(string sessionNodeId, uint subscriptionId, string nodeId)
    {
        MonitoredItem result = _monitoredItemManager.GetMonitoringItem(sessionNodeId, subscriptionId, nodeId);

        if (result == null)
        {
            return ApiResponse<MonitoredItem>.Failure(HttpStatusCode.NotFound);
        }

        return ApiResponse<MonitoredItem>.Success(result);
    }

    public async Task<MonitoredItemValue> FindMonitoringItemValue(Expression<Func<MonitoredItemValue, bool>> filter)
    {
        MonitoredItemValue result = await _monitoredItemValueRepo.FindAsync(filter, true);

        return result;
    }

    public ApiResponse<bool> DeleteMonitoringItem(string sessionNodeId, uint subscriptionId, string subscriptionGuid, string nodeId)
    {
        MonitoredItem monitoredItemOpc = _monitoredItemManager.DeleteMonitoringItem(sessionNodeId, subscriptionId, nodeId);

        Task monitoredItemDb = _monItemCfgRepo.DeleteAsync(c => c.StartNodeId == nodeId && c.SubscriptionGuid == subscriptionGuid);

        return ApiResponse<bool>.Success(true);
    }

    public ApiResponse<bool> UpdateMonitoredItem(string sessionNodeId, uint subscriptionId, MonitoredItemRequest updatedItem)
    {
        var result = _monitoredItemManager.UpdateMonitoredItem(sessionNodeId, updatedItem, subscriptionId);

        if (!result)
        {
            return ApiResponse<bool>.Failure(HttpStatusCode.NotFound);
        }
        // TODO: update in db
        return ApiResponse<bool>.Success(result);
    }


    public async Task<ApiResponse<MonitoredItemConfig>> UpdateSubscriptionEntityAsync(SubscriptionConfig subscriptionConfig, MonitoredItem monItem)
    {
        //prepare item
        MonitoredItemConfig monItemCfg = _mapper.Map<MonitoredItemConfig>(monItem);
        monItemCfg.SubscriptionGuid = subscriptionConfig.Guid;

        try
        {
            await _monItemCfgRepo.AddAsync(monItemCfg);
        }
        catch (Exception ex)
        {
            // tracking exception here
            LoggerManager.Logger.Error("{0}", ex);
        }

        return ApiResponse<MonitoredItemConfig>.Success(monItemCfg);
    }
}
