namespace QIA.Opc.Domain.Requests
{
	public class RequestObject
	{
		public string? SessionNodeId { get; set; }
		public string? SessionGuidId { get; set; }
		public string? SubscriptionGuidId { get; set; }
		public uint? OpcUaId { get; set; }
		public string? NodeId { get; set; }
		public object? NewValue { get; set; }
	}
}
