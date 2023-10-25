using Microsoft.AspNetCore.Mvc;
using Opc.Ua;
using Qia.Opc.Domain.DTO;
using Qia.Opc.Domain.Entities;
using Qia.Opc.Infrastrucutre.Services.OPCUA;

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
		public async Task<ActionResult<NodeReferenceEntity>> CreateSubscription([FromBody] NodeReferenceEntity nodeReference)
		{
			var nodeRef = await subscriptionService.SubscribeAsync(nodeReference);
			return Ok(nodeRef);
		}

		[HttpDelete("{subscriptionId}/{nodeId}")]
		public async Task<IActionResult> DeleteSubscription(string subscriptionId, string nodeId)
		{
			await subscriptionService.DeleteSubscriptionAsync(subscriptionId, nodeId);
			return Ok();
		}

		[HttpGet("list")]
		public ActionResult<SubscriptionDTO> GetActiveSubscriptions()
		{
			var items = subscriptionService.GetActiveSubscriptions();
			return Ok(items);
		}
	}
}
