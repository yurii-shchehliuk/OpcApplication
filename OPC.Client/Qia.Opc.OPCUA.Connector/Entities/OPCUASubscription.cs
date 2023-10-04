using Opc.Ua.Client;

namespace Qia.Opc.OPCUA.Connector.Entities
{
	public class OPCUASubscription
	{
		public string SubscriptionId { get; set; }
		public ISession Session { get; set; }
		public Subscription UaSubscription { get; set; }
	}


}
