using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Opc.Ua;
using Opc.Ua.Client;
using Qia.Opc.Domain.Common;
using Qia.Opc.Domain.Core;
using Qia.Opc.Domain.Entities;
using QIA.Opc.Domain.Entities;
using QIA.Opc.Domain.Repository;
using QIA.Opc.Domain.Requests;
using QIA.Opc.Domain.Responses;
using QIA.Opc.OPCUA.Connector.Managers;
using System.Threading;

namespace QIA.Opc.Infrastructure.Services.OPCUA
{
	public class MonitoredItemService
	{
		private readonly MonitoredItemManager monitoredItemManager;
		private readonly IMapper mapper;
		private readonly IGenericRepository<MonitoredItemConfig> monItemCfgRepo;
		private readonly IGenericRepository<MonitoredItemValue> monitoredItemValueRepo;

		public MonitoredItemService(MonitoredItemManager monitoredItemManager,
			IMapper mapper,
			IGenericRepository<MonitoredItemConfig> monitoredItemConfigRepo,
			IGenericRepository<MonitoredItemValue> monitoredItemValueRepo)
		{
			this.monitoredItemManager = monitoredItemManager;
			this.mapper = mapper;
			this.monItemCfgRepo = monitoredItemConfigRepo;
			this.monitoredItemValueRepo = monitoredItemValueRepo;
		}

		public ApiResponse<MonitoredItem> AddItemToSubscription(string sessionNodeId, uint subscriptionId, string nodeId)
		{
			try
			{
				if (!nodeId.TryParseNodeId(out var node))
					return ApiResponse<MonitoredItem>.Failure(HttpStatusCode.BadRequest, $"NodeId {nodeId} cannot be parsed");

				var item = monitoredItemManager.AddToSubscription(sessionNodeId, subscriptionId, nodeId);
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
				if (!nodeId.TryParseNodeId(out var node))
					return ApiResponse<MonitoredItem>.Failure(HttpStatusCode.BadRequest, $"NodeId {nodeId} cannot be parsed");

				var item = monitoredItemManager.AddToSubscription(sessionNodeId, subscription, nodeId);
				return ApiResponse<MonitoredItem>.Success(item);

			}
			catch (Exception ex)
			{
				return ApiResponse<MonitoredItem>.Failure(HttpStatusCode.BadRequest, $"Cannot add to the subscription:: {ex.Message} \n{ex.InnerException?.Message ?? ""}");
			}
		}

		public async Task SaveMonitoredItemValueAsync(MonitoredItemValue monitoredItemValue)
		{
			try
			{
				await monitoredItemValueRepo.AddAsync(monitoredItemValue);
			}
			catch (Exception ex)
			{
				LoggerManager.Logger.Error("Couldnt store item value to db: {0}", ex);
			}
		}

		public ApiResponse<MonitoredItem> GetMonitoringItem(string sessionNodeId, uint subscriptionId, string nodeId)
		{
			var result = monitoredItemManager.GetMonitoringItem(sessionNodeId, subscriptionId, nodeId);

			if (result == null) return ApiResponse<MonitoredItem>.Failure(HttpStatusCode.NotFound);

			return ApiResponse<MonitoredItem>.Success(result);
		}


		public ApiResponse<bool> DeleteMonitoringItem(string sessionNodeId, uint subscriptionId, string nodeId)
		{
			var result = monitoredItemManager.DeleteMonitoringItem(sessionNodeId, subscriptionId, nodeId);

			if (!result) return ApiResponse<bool>.Failure(HttpStatusCode.NotFound);
			// TODO: update in db

			return ApiResponse<bool>.Success(true);
		}

		public ApiResponse<bool> UpdateMonitoredItem(string sessionNodeId, uint subscriptionId, MonitoredItemRequest updatedItem)
		{
			var result = monitoredItemManager.UpdateMonitoredItem(sessionNodeId, updatedItem, subscriptionId);

			if (!result)
				return ApiResponse<bool>.Failure(HttpStatusCode.NotFound);
			// TODO: update in db
			return ApiResponse<bool>.Success(result);
		}


		public async Task<ApiResponse<MonitoredItemConfig>> UpdateSubscriptionEntityAsync(SubscriptionConfig subscriptionConfig, MonitoredItem monItem)
		{
			//prepare item
			var monItemCfg = mapper.Map<MonitoredItemConfig>(monItem);
			monItemCfg.SubscriptionGuidId = subscriptionConfig.SubscriptionGuidId;
			monItemCfg.GuidId = Guid.NewGuid().ToString();

			try
			{
				await monItemCfgRepo.AddAsync(monItemCfg);
			}
			catch (Exception ex)
			{
				// tracking exception here
				LoggerManager.Logger.Error("{0}", ex);
			}

			return ApiResponse<MonitoredItemConfig>.Success(monItemCfg);
		}
	}
}
