using Qia.Opc.Domain.Entities.Enums;
using Qia.Opc.Domain.Entities.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Qia.Opc.Domain.Entities
{
	public class SessionEntity : IBaseEntity
	{
		[Key]
		public int Id { get; set; }
		public string? SessionId { get; set; }
		public string? SessionNodeId { get; set; }
		public string Name { get; set; }
		public string EndpointUrl { get; set; }
		public SessionState State { get; set; } = SessionState.Disconnected;
		public ICollection<NodeReferenceEntity>? NodeConfigs { get; set; } = null;
	}
}
