namespace QIA.Opc.Domain.Requests
{
	public class RequestObject
	{
		public string? SessionNodeId { get; set; }
		public string? SessionGuid { get; set; }
		public string? SubscriptionGuid { get; set; }
		public uint? OpcUaId { get; set; }
		public string? NodeId { get; set; }
		public object? NewValue { get; set; }
	}
}
