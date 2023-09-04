using System.ComponentModel.DataAnnotations;

namespace SignalR.Hub.Entities
{
    /// <summary>
    /// DB node model
    /// </summary>
    public abstract class BaseNode
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string NodeId { get; set; }
        public string Name { get; set; }
        public NodeType NodeType { get; set; }
        public uint MSecs { get; set; } = 1000;
        public uint Range { get; set; } = 0;
    }

    /// <summary>
    /// outcome ndoe
    /// </summary>
    public class NodeData : BaseNode
    {
        public DateTime StoreTime { get; set; } = DateTime.UtcNow;
        public string Value { get; set; }
    }

    /// <summary>
    /// income node
    /// </summary>
    public class NewNode : BaseNode
    {
        public string Group { get; set; }
        public MonitorAction Action { get; set; }
    }

    public enum MonitorAction
    {
        Monitor,
        Unmonitor
    }
    public enum NodeType
    {
        Object,
        Method,
        Subscription,
        Value
    }

    public class TreeNode<T>
    {
        public T Data { get; set; }
        public HashSet<TreeNode<T>> Children { get; set; }

        public TreeNode(T value)
        {
            Data = value;
            Children = new HashSet<TreeNode<T>>();
        }
    }
}
