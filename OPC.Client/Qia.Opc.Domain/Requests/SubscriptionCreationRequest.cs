namespace Qia.Opc.Domain.Requests
{
	public class SubscriptionCreationRequest
	{
		public string SessionId { get; set; }
		public SubscriptionCreationOptions Options { get; set; }

	}
}
