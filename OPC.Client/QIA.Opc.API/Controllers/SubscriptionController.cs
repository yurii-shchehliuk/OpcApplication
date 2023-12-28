using Microsoft.AspNetCore.Mvc;
using Qia.Opc.Domain.Core;
using QIA.Opc.Domain.Entities;
using QIA.Opc.Domain.Requests;
using QIA.Opc.Domain.Responses;
using QIA.Opc.Infrastructure.Services.OPCUA;
using System.Web;

namespace QIA.Opc.API.Controllers
{
	public class SubscriptionController : BaseController
	{
		private readonly SubscriptionService subscriptionService;

		public SubscriptionController(SubscriptionService subscriptionService)
		{
			this.subscriptionService = subscriptionService;
		}

		[HttpPost("create")]
		public async Task<ActionResult<SubscriptionValue>> CreateSubscription([FromQuery] string nodeId, string sessionNodeId, [FromBody] SubscriptionRequest subsParams)
		{
			try
			{
				nodeId = HttpUtility.UrlDecode(nodeId);
				var response = await subscriptionService.SubscribeAsync(sessionNodeId, subsParams, nodeId);

				return HandleResponse(response);
			}
			catch (Exception ex)
			{
				LoggerManager.Logger.Error("{0}", ex);
				throw;
			}
		}

		[HttpDelete("delete/{subscriptionId}")]
		public async Task<IActionResult> DeleteSubscription([FromRoute] uint subscriptionId, [FromQuery] string sessionNodeId, string subscriptionGuidId)
		{
			try
			{
				var response = await subscriptionService.DeleteSubscriptionAsync(sessionNodeId, subscriptionId, subscriptionGuidId);

				return HandleResponse(response);
			}
			catch (Exception ex)
			{
				LoggerManager.Logger.Error("{0}", ex);
				throw;
			}
		}

		[HttpGet("{subscriptionId}")]
		public async Task<IActionResult> GetSubscriptionConfig([FromRoute] uint subscriptionId, [FromQuery] string sessionNodeId, string subscriptionGuidId)
		{
			try
			{
				var response = await subscriptionService.GetSubscriptionConfig(sessionNodeId, subscriptionId, subscriptionGuidId);

				return HandleResponse(response);
			}
			catch (Exception ex)
			{
				LoggerManager.Logger.Error("{0}", ex);
				throw;
			}
		}

		[HttpPut("modify")]
		public async Task<IActionResult> ModifySubscription([FromQuery] string sessionNodeId, [FromBody] SubscriptionRequest subsParams)
		{
			try
			{
				var response = await subscriptionService.ModifySubscriptionAsync(sessionNodeId, subsParams);

				return HandleResponse(response);
			}
			catch (Exception ex)
			{
				LoggerManager.Logger.Error("{0}", ex);
				throw;
			}
		}

		[HttpPut("setPublishingMode/{subscriptionId}")]
		public async Task<IActionResult> SetPublishingMode([FromRoute] uint subscriptionId, [FromQuery] string sessionNodeId, [FromBody] SubscriptionRequest request)
		{
			try
			{
				var response = subscriptionService.SetPublishingMode(sessionNodeId, subscriptionId, request.PublishingEnabled);
				// if subscription is not activated, create a new one from database
				if (!response.IsSuccess)
				{
					response = await subscriptionService.SubscribeFromModel(sessionNodeId, request);
				}
				// if creation form db fails, throw error

				return HandleResponse(response);
			}
			catch (Exception ex)
			{
				LoggerManager.Logger.Error("{0}", ex);
				throw;
			}
		}

		[HttpGet("list")]
		public async Task<IActionResult> GetSubscriptions([FromQuery] string sessionNodeId)
		{
			try
			{
				var response = await subscriptionService.GetSubscriptions(sessionNodeId);

				return HandleResponse(response);
			}
			catch (Exception ex)
			{
				LoggerManager.Logger.Error("{0}", ex);
				throw;
			}
		}
	}
}
