using Qia.Opc.Domain.Entities.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Qia.Opc.Domain.Entities
{
	public class SessionEntity : IBaseEntity
	{
		[Key]
		public int Id { get; set; }
		public string SessionId { get; set; }
		public string Name { get; set; }
		public string EndpointUrl { get; set; }
		public ICollection<NodeReferenceEntity>? NodeConfigs { get; set; } = null;
	}
}
