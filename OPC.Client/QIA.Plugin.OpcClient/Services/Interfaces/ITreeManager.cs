using QIA.Plugin.OpcClient.Core.Settings;
using QIA.Plugin.OpcClient.DTOs;
using QIA.Plugin.OpcClient.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QIA.Plugin.OpcClient.Services.Interfaces
{
    public interface ITreeManager
    {
        TreeNode<NodeData> BrowseTree();

        TreeNode<NodeData> FindNodesRecursively(IAppSettings appSettings);

        TreeNode<T> SearchAndBuild<T>(TreeNode<T> root, List<T> itemsToFind);

        Task WriteTree(string graphName, TreeNode<NodeData> treeData);
    }
}