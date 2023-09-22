using QIA.Plugin.OpcClient.Core;
using QIA.Plugin.OpcClient.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QIA.Plugin.OpcClient.Services.Interfaces
{
    public interface ITreeManager
    {
        TreeNode BrowseTree();

        Task<TreeNode> FindNodesRecursively(string group = "All");

        Task WriteTree(string graphName, TreeNode treeData);
    }
}