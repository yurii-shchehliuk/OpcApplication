using Qia.Opc.Domain.Entities.Interfaces;

namespace Qia.Opc.Domain.Entities
{
	public class Group : IBaseEntity
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int? AppSettingsId { get; set; }
		public virtual AppSettings AppSettings { get; set; }
	}
}
