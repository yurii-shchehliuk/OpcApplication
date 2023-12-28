using Qia.Opc.Domain.Entities.Interfaces;

namespace Qia.Opc.Domain.Entities
{
	/// <summary>
	/// monitored node value
	/// </summary>
	public class MonitoredItemValue : IBaseEntity
	{
		public int ServerId { get; set; }
		public uint SubscriptionOpcId { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public string DisplayName { get; set; }
		public string StartNodeId { get; set; }
		public int SamplingInterval { get; set; }
		public uint QueueSize { get; set; }
		public string? Value { get; set; }
	}
}
