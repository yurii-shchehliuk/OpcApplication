using Qia.Opc.Domain.Entities.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Qia.Opc.Domain.Entities
{
	/// <summary>
	/// node values 
	/// </summary>
	public class NodeValue : IBaseEntity
	{
		public int Id { get; set; }
		public string NodeId { get; set; }
		public string? SessionName { get; set; }
		public string DisplayName { get; set; }
		public uint? MSecs { get; set; }
		public uint? Range { get; set; }
		public DateTime? StoreTime { get; set; }
		public string? Value { get; set; }
	}
}
