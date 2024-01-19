using Opc.Ua;
using Qia.Opc.Domain.Entities.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Qia.Opc.Domain.Entities
{
	/// <summary>
	/// node reference 
	/// </summary>
	public class MonitoredItemConfig : IBaseEntity
	{
		[Key]
		public string Guid { get; set; } = System.Guid.NewGuid().ToString();
		public int OpcUaId { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
		public string DisplayName { get; set; }
		public string StartNodeId { get; set; }
		public int SamplingInterval { get; set; }
		public uint QueueSize { get; set; }
		public bool DiscardOldest { get; set; }
		public NodeClass NodeClass { get; set; }
		public uint AttributeId { get; set; }
		public string? RelativePath { get; set; }
		public string? IndexRange { get; set; }
		public MonitoringMode MonitoringMode { get; set; }
		//
		public string SubscriptionGuid { get; set; }
	}
}
