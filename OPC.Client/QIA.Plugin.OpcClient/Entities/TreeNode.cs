using System;
using System.Collections.Generic;

namespace QIA.Plugin.OpcClient.Entities
{
    /// <summary>
    /// UI nodes graph
    /// </summary>
    public class TreeNode 
    {
        public string NodeId { get; set; }
        public string Name { get; set; }
        public HashSet<TreeNode> Children { get; set; } = new HashSet<TreeNode>();
    }
}