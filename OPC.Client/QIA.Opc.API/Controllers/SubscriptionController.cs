using Microsoft.AspNetCore.Mvc;
using Opc.Ua;
using Qia.Opc.Domain.Common;
using Qia.Opc.Domain.Entities;
using QIA.Opc.Domain.Request;
using QIA.Opc.Domain.Response;
using QIA.Opc.Infrastructure.Services.OPCUA;

namespace QIA.Opc.API.Controllers
{
	public class SubscriptionController : BaseController
	{
		private readonly SubscriptionService subscriptionService;

		public SubscriptionController(SubscriptionService subscriptionService)
		{
			this.subscriptionService = subscriptionService;
		}

		[HttpPost("create/{nodeId}")]
		public async Task<IActionResult> CreateSubscription([FromBody] SubscriptionParameters subsParams, string nodeId)
		{
			if (!nodeId.TryParseNodeId(out var node))
				return NotFound("NodeId cannot be parsed");

			await subscriptionService.SubscribeAsync(subsParams, nodeId);
			return Ok();
		}

		[HttpPut("addToSubscription/{subscriptionName}/{nodeId}")]
		public async Task<IActionResult> AddToSubscription(string subscriptionName, string nodeId)
		{
			if (!nodeId.TryParseNodeId(out var node))
				return NotFound("NodeId cannot be parsed");

			await subscriptionService.AddToSubscription(subscriptionName, nodeId);

			return Ok();
		}

		[HttpDelete("{subscriptionId}")]
		public async Task<IActionResult> DeleteSubscription(uint subscriptionId)
		{
			await subscriptionService.DeleteSubscriptionAsync(subscriptionId);
			return Ok();
		}

		[HttpDelete("deleteMonitoringItem/{subscriptionId}/{nodeId}")]
		public IActionResult DeleteMonitoringItem(uint subscriptionId, string nodeId)
		{
			subscriptionService.DeleteMonitoringItem(subscriptionId, nodeId);
			return Ok();
		}

		[HttpPut("modify/{subscriptionId}")]
		public IActionResult ModifySubscription([FromBody] SubscriptionParameters subsParams, uint subscriptionId)
		{
			subscriptionService.ModifySubscription(subsParams, subscriptionId);
			return Ok();
		}

		[HttpPut("setPublishingMode/{subscriptionId}/{enable}")]
		public IActionResult SetPublishingMode(string subscriptionId, bool enable)
		{
			subscriptionService.SetPublishingMode(subscriptionId, enable);

			return Ok();
		}

		[HttpGet("list")]
		public async Task<IActionResult> GetActiveSubscriptions()
		{
			await subscriptionService.GetActiveSubscriptions();
			return Ok();
		}
	}
}
