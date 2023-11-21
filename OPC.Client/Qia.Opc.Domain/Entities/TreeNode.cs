using Opc.Ua;
using Qia.Opc.Domain.Entities.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Qia.Opc.Domain.Entities
{
	public class TreeContainer : IBaseEntity
	{
		public int Id { get; set; }
		public string SourceName { get; set; }
		public string Data { get; set; }
		public DateTime StoreTime { get; set; }
	}

	public class TreeNode
	{
		public string NodeId { get; set; }
		public string DisplayName { get; set; }
		public NodeClass NodeClass { get; set; }
		public HashSet<TreeNode>? Children { get; set; } =
		new HashSet<TreeNode>();
	}
}
