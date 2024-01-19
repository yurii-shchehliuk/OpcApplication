using Qia.Opc.Domain.Entities.Enums;
using Qia.Opc.Domain.Entities.Interfaces;
using QIA.Opc.Domain.Entities;

namespace Qia.Opc.Domain.Entities
{
	public class SessionEntity : IBaseEntity
	{
		public string Guid { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
		public DateTime? LastAccessed { get; set; }
		public string Name { get; set; }
		public string EndpointUrl { get; set; }
		public SessionState State { get; set; } = SessionState.Disconnected;
		public List<SubscriptionConfig>? SubscriptionConfigs { get; set; } = new();
	}
}
