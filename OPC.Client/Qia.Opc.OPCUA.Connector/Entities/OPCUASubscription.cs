using Opc.Ua.Client;

namespace Qia.Opc.OPCUA.Connector.Entities
{
	public class OPCUASubscription
	{
		public string SubscriptionGuidId { get; set; }
		public Subscription Subscription { get; set; }
	}
}
