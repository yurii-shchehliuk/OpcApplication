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
			var nodeRef = subscriptionService.Subscribe(nodeReference);
			return Ok(nodeRef);
		}

		[HttpDelete("{subscriptionId}")]
		public IActionResult DeleteSubscription(string subscriptionId)
		{
			subscriptionService.DeleteSubscription(subscriptionId);
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
