namespace QIA.Opc.API.Controllers;

using System.Web;
using Microsoft.AspNetCore.Mvc;
using QIA.Opc.Application.Requests;
using QIA.Opc.Application.Responses;
using QIA.Opc.Domain.Entities;
using QIA.Opc.Infrastructure.Services.OPCUA;

public class SubscriptionController : BaseController
{
    private readonly SubscriptionService _subscriptionService;

    public SubscriptionController(SubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    [HttpPost("create")]
    public async Task<ActionResult<SubscriptionValue>> CreateSubscription([FromQuery] string nodeId, string sessionNodeId, [FromBody] SubscriptionRequest subsParams)
    {
        nodeId = HttpUtility.UrlDecode(nodeId);
        Infrastructure.Application.ApiResponse<SubscriptionValue> response = await _subscriptionService.SubscribeAsync(sessionNodeId, subsParams, nodeId);

        return HandleResponse(response);
    }

    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<SubscriptionValue>>> GetSubscriptions([FromQuery] string sessionNodeId)
    {
        Infrastructure.Application.ApiResponse<IEnumerable<SubscriptionValue>> response = await _subscriptionService.GetSubscriptions(sessionNodeId);

        return HandleResponse(response);
    }

    [HttpGet]
    public async Task<ActionResult<SubscriptionConfig>> GetSubscriptionConfig([FromQuery] string subscriptionGuid)
    {
        Infrastructure.Application.ApiResponse<SubscriptionConfig> response = await _subscriptionService.GetSubscriptionConfig(subscriptionGuid);

        return HandleResponse(response);
    }

    [HttpPut("modify")]
    public async Task<ActionResult<SubscriptionConfig>> ModifySubscription([FromQuery] string sessionNodeId, [FromBody] SubscriptionRequest subsParams)
    {
        Infrastructure.Application.ApiResponse<SubscriptionConfig> response = await _subscriptionService.ModifySubscriptionAsync(sessionNodeId, subsParams);

        return HandleResponse(response);
    }

    [HttpPut("setPublishingMode/{subscriptionId}")]
    public async Task<ActionResult<SubscriptionValue>> SetPublishingMode([FromRoute] uint subscriptionId, [FromQuery] string sessionNodeId, [FromBody] SubscriptionRequest request)
    {
        if (subscriptionId == 0)
        {
            // subscription is not activated, create a new one from database
            Infrastructure.Application.ApiResponse<SubscriptionValue> subscribeFromDbModelResponse = await _subscriptionService.SubscribeFromDbModel(sessionNodeId, request);
            return HandleResponse(subscribeFromDbModelResponse);
        }

        // subscription been activated 
        Infrastructure.Application.ApiResponse<SubscriptionValue> setPublishResponse = _subscriptionService.SetPublishingMode(sessionNodeId, subscriptionId, request);

        return HandleResponse(setPublishResponse);
    }

    [HttpPut("stopAll")]
    public IActionResult StopAllSubscriptions([FromQuery] string sessionNodeId)
    {
        Infrastructure.Application.ApiResponse<bool> response = _subscriptionService.StopAllSubscriptions(sessionNodeId);

        return HandleResponse(response);
    }

    [HttpDelete("delete/{subscriptionId}")]
    public async Task<IActionResult> DeleteSubscription([FromRoute] uint subscriptionId, [FromQuery] string sessionNodeId, string subscriptionGuid)
    {
        Infrastructure.Application.ApiResponse<bool> response = await _subscriptionService.DeleteSubscriptionAsync(sessionNodeId, subscriptionId, subscriptionGuid);

        return HandleResponse(response);
    }
}
