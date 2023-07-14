using Microsoft.EntityFrameworkCore.Internal;
using Opc.Ua;
using Opc.Ua.Client;
using QIA.Plugin.OpcClient.Core;
using QIA.Plugin.OpcClient.DTOs;
using QIA.Plugin.OpcClient.Entities;
using QIA.Plugin.OpcClient.Repository;
using QIA.Plugin.OpcClient.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace QIA.Plugin.OpcClient.Services
{
    public class TreeManager : ITreeManager
    {
        private readonly Session m_session;
        private readonly IDataAccess _repo;
        private readonly ISubscriptionManager _sm;
        private readonly INodeManager _nm;
        private readonly IAzureMessageService _azureMS;
        private static HashSet<NodeConfig> itemsToFind = null;

        public TreeManager(IDataAccess repo, ISubscriptionManager sm, INodeManager nm, IAzureMessageService azureMS)
        {
            m_session = AppConfiguration.OpcUaClientSession;
            _repo = repo;
            _sm = sm;
            _nm = nm;
            _azureMS = azureMS;
            try
            {
                itemsToFind = JsonSerializer.Deserialize<HashSet<NodeConfig>>(File.ReadAllText("qia.opc/nodemanager.json"));
            }
            catch (Exception ex)
            {
                LoggerManager.Logger.Error(ex, "Couldn't deserialize nodemanager:");
                throw new Exception("Couldn't deserialize nodemanager", ex.InnerException);
            }
        }

        /// <summary>
        /// Browse server's data tree
        /// </summary>
        public TreeNode<SampleNode> BrowseTree()
        {
            m_session.Browse(null, null, ObjectIds.ObjectsFolder, 0u, BrowseDirection.Forward,
                        ReferenceTypeIds.HierarchicalReferences, true, (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method, out byte[] nextCp, out ReferenceDescriptionCollection nextRefs);

            int deepLvl = -1;
            string parentName = "ROOT";

            var treeNode = new TreeNode<SampleNode>(new()
            {
                Name = parentName,
                NodeId = deepLvl.ToString()
            });

            var appConfig = _repo.GetAppConfig();

            Console.WriteLine($"AppConfig.json:" +
                    JsonSerializer.Serialize(appConfig,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web)
                    {
                        WriteIndented = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,

                    }).Replace("\"", ""));

            if (!appConfig.CreateFullTree)
                return treeNode;

            Console.WriteLine("DisplayName  |  BrowseName  |  NodeClass\n-------------+--------------+-----------");
            // browse all the available nodes
            foreach (var rd in nextRefs)
            {
                /// skip predefined
                if (appConfig.SkipPredefined
                    && (rd.DisplayName.Text == "Server" || rd.DisplayName.Text == "Aliases"))
                    continue;

                BrowseFoldersFunc(rd, treeNode.Children);
            }

            return treeNode;

            void BrowseFoldersFunc(ReferenceDescription rd, HashSet<TreeNode<SampleNode>> treeNodeList)
            {
                deepLvl++;
                string currentName = rd.DisplayName.Text;
                string currentNodeId = rd.NodeId.Identifier.ToString();
                Console.WriteLine($"[{deepLvl} - {parentName}] {rd.DisplayName}  |   {rd.BrowseName}  |   {rd.NodeClass}   |   {rd.NodeId}");

                /// skip if already found
                var valueExist = treeNodeList.FirstOrDefault(c => c.Data.NodeId == currentNodeId);
                if (valueExist != null)
                    return;

                /// add same row children
                treeNodeList.Add(new TreeNode<SampleNode>(new()
                {
                    NodeId = currentNodeId,
                    Name = currentName,
                    Value = _nm.ReadNodeValue(rd.NodeId)
                }));

                m_session.Browse(null, null, ExpandedNodeId.ToNodeId(rd.NodeId, m_session.NamespaceUris), 0u, BrowseDirection.Forward,
                            ReferenceTypeIds.HierarchicalReferences, true, (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method, out byte[] nextCp, out ReferenceDescriptionCollection nextRefs);

                /// next row children
                if (nextRefs != null && nextRefs.Count > 0)
                {
                    foreach (var item in nextRefs)
                    {
                        parentName = rd.DisplayName.Text;
                        var currentNode = treeNodeList.FirstOrDefault(c => c.Data.NodeId == rd.NodeId.Identifier.ToString());
                        BrowseFoldersFunc(item, currentNode.Children);
                    }
                }
                deepLvl--;
            }
        }

        public TreeNode<SampleNode> FindNodesRecursively(HashSet<NodeConfig> itemsToFind = null)
        {
            int deepLvl = -1;
            string parentName = "ROOT";
            itemsToFind ??= TreeManager.itemsToFind;

            HashSet<NodeConfig> notFoundItems = new(itemsToFind); ///scince we are checking HasNeededChild we need a copy
            TreeNode<SampleNode> treeNode = new(new()
            {
                NodeId = (deepLvl + 1).ToString(),
                Name = parentName,
            });

            m_session.Browse(null, null, ObjectIds.ObjectsFolder, 0u, BrowseDirection.Forward,
            ReferenceTypeIds.HierarchicalReferences, true, (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method, out byte[] nextCp, out ReferenceDescriptionCollection nextRefs);
            Console.WriteLine();
            LoggerManager.Logger.Information("Searching started");

            int foundCounter = 0;
            LoggerManager.Logger.Information("Found {0} nodes", foundCounter);
            int lineToUpdate = Console.CursorTop;

            Console.WriteLine("DisplayName  |  BrowseName  |  NodeClass\n-------------+--------------+-----------");
            // browse all the available nodes
            foreach (var rd in nextRefs)
            {
                /// if mssql installed and
                if (_repo.GetAppConfig().SkipPredefined
                && (rd.DisplayName.Text == "Server" || rd.DisplayName.Text == "Aliases"))
                    continue;

                FindNodesRecursivelyFunc(rd, treeNode.Children);
            }

            foreach (var item in notFoundItems)
            {
                LoggerManager.Logger.Error("404: {0} | {1}", item.NodeId, item.Name);
            }
            return treeNode;

            void FindNodesRecursivelyFunc(ReferenceDescription rd, HashSet<TreeNode<SampleNode>> treeNodeList)
            {
                deepLvl++;
                bool valueFound = false;
                string currentNodeId = rd.NodeId.Identifier.ToString();

                /// add same row children
                var currentNode = new TreeNode<SampleNode>(new SampleNode
                {
                    NodeId = currentNodeId,
                    Name = rd.DisplayName.Text,
                    Value = null
                });

                /// skip if already found
                var valueExist = treeNodeList.FirstOrDefault(c => c.Data.NodeId == currentNodeId);
                if (valueExist != null)
                    return;

                treeNodeList.Add(currentNode);

                var foundItem = itemsToFind?.FirstOrDefault(c => c.NodeId == currentNodeId);
                if (foundItem is not null)
                {
                    foundCounter++;
                    LoggerManager.UpdateConsoleLine(lineToUpdate, string.Format("Found {0} nodes", foundCounter));

                    Console.WriteLine($"[{deepLvl} - {parentName}] {rd.DisplayName}  |   {rd.BrowseName}  |   {rd.NodeClass}   |   {rd.NodeId}");
                    // new tree to be updated
                    valueFound = true;
                    notFoundItems.Remove(foundItem);

                    var node = treeNodeList.FirstOrDefault(currentNode);
                    node.Data.Value = _nm.ReadNodeValue(rd.NodeId);
                    node.Data.NodeType = foundItem.NodeType;
                    node.Data.MSecs = foundItem.Msecs;
                    node.Data.Range = foundItem.Range;

                    _repo.AddAsync(node.Data).Wait();

                    // subscription mng
                    if (foundItem.NodeType == NodeType.Subscription)
                    {
                        _sm.SubscribeNew(rd);
                    }

                    _azureMS.SendMessageAsync(node.Data);
                }

                m_session.Browse(null, null, ExpandedNodeId.ToNodeId(rd.NodeId, m_session.NamespaceUris), 0u, BrowseDirection.Forward,
                            ReferenceTypeIds.HierarchicalReferences, true, (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method, out byte[] nextCp, out ReferenceDescriptionCollection nextRefs);

                /// next row children
                if (nextRefs != null && nextRefs.Count > 0)
                {
                    foreach (var nextRd in nextRefs)
                    {
                        parentName = rd.DisplayName.Text;
                        // Get parent in the new tree
                        var childNode = treeNodeList.FirstOrDefault(c => c.Data.NodeId == currentNodeId.ToString());
                        FindNodesRecursivelyFunc(nextRd, childNode.Children);
                    }
                }
                /// do current parent has needed child
                bool hasNeeded = HasNeededChild(treeNodeList.FirstOrDefault(c => c.Data.NodeId == currentNodeId).Children, itemsToFind);
                if (!valueFound && !hasNeeded || nextRefs.Count == 0 && !valueFound)
                {
                    treeNodeList.Remove(currentNode);
                }

                deepLvl--;
            }

            bool HasNeededChild(HashSet<TreeNode<SampleNode>> treeNode, HashSet<NodeConfig> itemsToFind = null)
            {
                foreach (var itemTree in treeNode)
                {
                    if (itemsToFind.Any(c => c.NodeId == itemTree.Data.NodeId))
                    {
                        return true;
                    }

                    if (itemTree.Children.Any())
                    {
                        return HasNeededChild(itemTree.Children, itemsToFind);
                    }
                }

                return false;
            }
        }

        public TreeNode<T> SearchAndBuild<T>(TreeNode<T> root, List<T> itemsToFind)
        {
            if (root == null)
                return null;

            // A dictionary to keep track of corresponding nodes in the new tree
            Dictionary<TreeNode<T>, TreeNode<T>> nodeMapping = new Dictionary<TreeNode<T>, TreeNode<T>>();

            Queue<TreeNode<T>> queue = new Queue<TreeNode<T>>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (itemsToFind.Contains(current.Data))
                {
                    TreeNode<T> newNode = new TreeNode<T>(current.Data);
                    nodeMapping[current] = newNode;

                    if (current != root)
                    {
                        // Get parent in the new tree
                        var parent = nodeMapping.Values.First(x => x.Children.Contains(current));
                        parent.Children.Add(newNode);
                    }
                }

                foreach (var child in current.Children)
                {
                    queue.Enqueue(child);
                }
            }

            // The root of the new tree is the node corresponding to the root of the original tree
            return nodeMapping.ContainsKey(root) ? nodeMapping[root] : null;
        }

        public static NodeConfig FindNode(NodeId node)
        {
            return itemsToFind.FirstOrDefault(c => c.NodeId == node.Identifier.ToString());
        }
    }
}
