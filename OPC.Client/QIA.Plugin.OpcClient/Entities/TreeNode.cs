using System.Collections.Generic;

namespace QIA.Plugin.OpcClient.Entities
{
    /// <summary>
    /// Nodes graph
    /// </summary>
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