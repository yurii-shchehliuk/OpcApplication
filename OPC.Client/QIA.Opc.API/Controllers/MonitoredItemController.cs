using Microsoft.AspNetCore.Mvc;
using Qia.Opc.Domain.Core;
using Qia.Opc.Domain.Entities;
using QIA.Opc.Domain.Requests;
using QIA.Opc.Infrastructure.Services.OPCUA;
using System.Web;

namespace QIA.Opc.API.Controllers
{
	public class MonitoredItemController : BaseController
	{
		private readonly MonitoredItemService monitoredItemService;
		private readonly SubscriptionService subscriptionService;

		public MonitoredItemController(MonitoredItemService monitoredItemService, SubscriptionService subscriptionService)
		{
			this.monitoredItemService = monitoredItemService;
			this.subscriptionService = subscriptionService;
		}

		[HttpPost("addToSubscription")]
		public async Task<IActionResult> AddToSubscription([FromBody] RequestObject requestObj)
		{
			try
			{
				var monitoredItem = monitoredItemService.AddItemToSubscription(requestObj.SessionNodeId, (uint)requestObj.OpcUaId, requestObj.NodeId);
				if (!monitoredItem.IsSuccess)
				{
					return HandleResponse(monitoredItem);
				}

				var subscriptionConfig = await subscriptionService.GetSubscriptionConfig(requestObj.SessionNodeId, (uint)requestObj.OpcUaId);
				if (!subscriptionConfig.IsSuccess)
				{
					return HandleResponse(subscriptionConfig);
				}

				var item = await monitoredItemService.UpdateSubscriptionEntityAsync(subscriptionConfig.Value, monitoredItem.Value);

				return HandleResponse(item);
			}
			catch (Exception ex)
			{
				LoggerManager.Logger.Error("{0}", ex);
				throw;
			}
		}

		[HttpGet("get")]
		public IActionResult GetMonitoredItem([FromQuery] string nodeId, string sessionNodeId, uint subscriptionId)
		{
			try
			{
				nodeId = HttpUtility.UrlDecode(nodeId);
				var response = monitoredItemService.GetMonitoringItem(sessionNodeId, subscriptionId, nodeId);

				return HandleResponse(response);
			}
			catch (Exception ex)
			{
				LoggerManager.Logger.Error("{0}", ex);
				throw;
			}
		}

		[HttpDelete("delete")]
		public IActionResult DeleteMonitoringItem([FromQuery] string nodeId, string sessionNodeId, uint subscriptionId)
		{
			try
			{
				nodeId = HttpUtility.UrlDecode(nodeId);
				var response = monitoredItemService.DeleteMonitoringItem(sessionNodeId, subscriptionId, nodeId);

				return HandleResponse(response);
			}
			catch (Exception ex)
			{
				LoggerManager.Logger.Error("{0}", ex);
				throw;
			}
		}

		[HttpPut("update")]
		public IActionResult UpdateMonitoredItem([FromQuery] string sessionNodeId, uint subscriptionId, [FromBody] MonitoredItemRequest updatedItem)
		{
			try
			{
				var updatedSessionResponse = monitoredItemService.UpdateMonitoredItem(sessionNodeId, subscriptionId, updatedItem);

				return HandleResponse(updatedSessionResponse);
			}
			catch (Exception ex)
			{
				LoggerManager.Logger.Error("{0}", ex);
				throw;
			}
		}
	}
}
