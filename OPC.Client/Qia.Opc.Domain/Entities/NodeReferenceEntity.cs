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
		public string? SubscriptionId { get; set; }
		public uint? MSecs { get; set; }
		public uint? Range { get; set; }
	}
}
