using Opc.Ua;
using Qia.Opc.Domain.Entities.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Qia.Opc.Domain.Entities
{
	/// <summary>
	/// UI nodes graph
	/// </summary>
	public abstract class TreeNodeBase
	{
		public string NodeId { get; set; }
		public string DisplayName { get; set; }
		public NodeClass NodeClass { get; set; }
	}

	public class TreeNode : TreeNodeBase
	{
		public HashSet<TreeNode>? Children { get; set; }
	}
}
