using Opc.Ua;
using Qia.Opc.Domain.Entities.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Qia.Opc.Domain.Entities
{
	/// <summary>
	/// node reference 
	/// </summary>
	public class NodeReferenceEntity : IBaseNode
	{
		public string NodeId { get; set; }
		public string DisplayName { get; set; }
		public NodeClass NodeClass { get; set; }
		public string? SubscriptionId { get; set; }
		public uint? MSecs { get; set; }
		public int? SessionEntityId { get; set; }
		public SessionEntity? SessionEntity { get; set; }
	}
}
