using QIA.Plugin.OpcClient.DTOs;
using QIA.Plugin.OpcClient.Entities;
using System.Collections.Generic;

namespace QIA.Plugin.OpcClient.Services.Interfaces
{
    public interface ITreeManager
    {
        TreeNode<SampleNode> BrowseTree();

        TreeNode<SampleNode> FindNodesRecursively(HashSet<NodeConfig> itemsToFind = null);

        TreeNode<T> SearchAndBuild<T>(TreeNode<T> root, List<T> itemsToFind);
    }
}