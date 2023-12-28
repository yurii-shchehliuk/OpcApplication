using Opc.Ua;
using Qia.Opc.Domain.Entities.Interfaces;

namespace Qia.Opc.Domain.Entities
{
	public class TreeContainer : IBaseEntity
	{
		public int Id { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
		public string SourceName { get; set; }
		public string Data { get; set; }
	}

	public class TreeNode
	{
		public string StartNodeId { get; set; }
		public string DisplayName { get; set; }
		public NodeClass NodeClass { get; set; }
		public HashSet<TreeNode>? Children { get; set; } =
		new HashSet<TreeNode>();
	}
}
