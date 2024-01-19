using Qia.Opc.Domain.Entities;

namespace QIA.Opc.Domain.Responses
{
	public class SubscriptionValue
	{
		public string Guid { get; set; }
		public uint OpcUaId { get; set; } = 0;
		public string DisplayName { get; set; }
		public int PublishingInterval { get; set; }
		public uint ItemsCount { get; set; }
		public uint SequenceNumber { get; set; } = 0;
		public string SessionNodeId { get; set; }
		public bool PublishingEnabled { get; set; } = false;
		public List<MonitoredItemValue> MonitoredItems { get; set; } = new List<MonitoredItemValue>();
	}
}
