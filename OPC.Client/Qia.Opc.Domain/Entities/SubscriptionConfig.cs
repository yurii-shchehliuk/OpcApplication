using Qia.Opc.Domain.Entities;
using Qia.Opc.Domain.Entities.Interfaces;

namespace QIA.Opc.Domain.Entities
{
	/// <summary>
	/// Requested subscription
	/// </summary>
	public class SubscriptionConfig : IBaseEntity
	{
		public string SubscriptionGuidId { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
		public string DisplayName { get; set; } = string.Empty;
		public int PublishingInterval { get; set; } = 1000;
		public uint KeepAliveCount { get; set; } = 10;
		public uint LifetimeCount { get; set; } = 1000;
		public uint MaxNotificationsPerPublish { get; set; } = 0;
		public byte Priority { get; set; } = 255;
		//
		public string? SessionGuidId { get; set; }
		public SessionEntity? Session { get; set; }
		public List<MonitoredItemConfig> MonitoredItemsConfig { get; set; } = new();
	}
}
