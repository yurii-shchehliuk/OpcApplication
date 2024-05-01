namespace QIA.Opc.API.Controllers;

using System.Web;
using global::Opc.Ua.Client;
using Microsoft.AspNetCore.Mvc;
using Qia.Opc.Domain.Entities;
using QIA.Opc.Application.Requests;
using QIA.Opc.Domain.Entities;
using QIA.Opc.Infrastructure.Application;
using QIA.Opc.Infrastructure.Services.OPCUA;

public class MonitoredItemController : BaseController
{
    private readonly MonitoredItemService _monitoredItemService;
    private readonly SubscriptionService _subscriptionService;

    public MonitoredItemController(MonitoredItemService monitoredItemService, SubscriptionService subscriptionService)
    {
        _monitoredItemService = monitoredItemService;
        _subscriptionService = subscriptionService;
    }

    [HttpPost("addToSubscription")]
    public async Task<IActionResult> AddToSubscription([FromQuery]string sessionNodeId,string subscriptionGuid, uint subscriptionId,[FromBody] string nodeId)
    {
        ApiResponse<MonitoredItem> monitoredItem = _monitoredItemService.AddItemToSubscription(sessionNodeId,
            subscriptionId,
            nodeId);

        if (!monitoredItem.IsSuccess)
        {
            return HandleResponse(monitoredItem);
        }

        ApiResponse<SubscriptionConfig> subscriptionConfig = await _subscriptionService.GetSubscriptionConfig(subscriptionGuid);
        if (!subscriptionConfig.IsSuccess)
        {
            return HandleResponse(subscriptionConfig);
        }

        ApiResponse<MonitoredItemConfig> item = await _monitoredItemService.UpdateSubscriptionEntityAsync(subscriptionConfig.Value, monitoredItem.Value);

        return HandleResponse(item);

    }

    [HttpGet("get")]
    public IActionResult GetMonitoredItem([FromQuery] string sessionNodeId, uint subscriptionId, string nodeId)
    {
        try
        {
            nodeId = HttpUtility.UrlDecode(nodeId);
            ApiResponse<MonitoredItem> response = _monitoredItemService.GetMonitoringItem(sessionNodeId, subscriptionId, nodeId);

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

        ApiResponse<bool> updatedSessionResponse = _monitoredItemService.UpdateMonitoredItem(sessionNodeId, subscriptionId, updatedItem);

        return HandleResponse(updatedSessionResponse);

    }

    [HttpDelete("delete")]
    public IActionResult DeleteMonitoringItem([FromQuery] string sessionNodeId, uint subscriptionId, string subscriptionGuid, string nodeId)
    {

        nodeId = HttpUtility.UrlDecode(nodeId);
        ApiResponse<bool> response = _monitoredItemService.DeleteMonitoringItem(sessionNodeId, subscriptionId, subscriptionGuid, nodeId);

        return HandleResponse(response);

    }
}
