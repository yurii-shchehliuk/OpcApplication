using Opc.Ua;

namespace QIA.Opc.Domain.Requests
{
	public class MonitoredItemRequest
	{
		public string DisplayName { get; set; }
		public string StartNodeId { get; set; }
		public int SamplingInterval { get; set; }
		public uint QueueSize { get; set; }
		public bool DiscardOldest { get; set; }
		public uint AttributeId { get; set; }
		public string? RelativePath { get; set; }
		public string? IndexRange { get; set; }
		public NodeClass NodeClass { get; set; }
		public MonitoringMode MonitoringMode { get; set; }
	}
}
