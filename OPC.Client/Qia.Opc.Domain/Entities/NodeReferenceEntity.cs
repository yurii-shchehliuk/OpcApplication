using Opc.Ua;
using Qia.Opc.Domain.Entities.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Qia.Opc.Domain.Entities
{
	/// <summary>
	/// node reference 
	/// </summary>
	public class NodeReferenceEntity : TreeNodeBase, IBaseNode
	{
#pragma warning disable CS8632
		public string? SubscriptionId { get; set; }
		public uint? MSecs { get; set; }
		public uint? Range { get; set; }
		public int? SessionEntityId { get; set; }
		public SessionEntity? SessionEntity { get; set; }
#pragma warning restore CS8632
	}
}
