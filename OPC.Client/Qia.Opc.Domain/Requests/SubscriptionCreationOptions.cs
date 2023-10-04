namespace Qia.Opc.Domain.Requests
{
	public class SubscriptionCreationOptions
	{
		public int PublishingInterval { get; internal set; }
		public uint KeepAliveCount { get; internal set; }
	}
}
