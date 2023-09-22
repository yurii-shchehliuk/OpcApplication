using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using Opc.Ua;
using Opc.Ua.Client;
using QIA.Plugin.OpcClient.Core;
using QIA.Plugin.OpcClient.Entities;
using QIA.Plugin.OpcClient.Repository;
using QIA.Plugin.OpcClient.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QIA.Plugin.OpcClient.Services
{
    public class TreeManager : ITreeManager
	{
		private Session m_session;
		private readonly IDataAccess<NodeData> _repo;
		private readonly ISubscriptionManager _sm;
		private readonly INodeManager _nm;
		private readonly IAzureMessageService _azureMS;

		/// <summary>
		/// data storage from appSettings
		/// </summary>
		private static HashSet<BaseNode> itemsToFind = null;

		public TreeManager(IDataAccess<NodeData> repo, ISubscriptionManager sm, INodeManager nm, IAzureMessageService azureMS)
		{
			m_session = OpcConfiguration.OpcUaClientSession;
			_repo = repo;
			_sm = sm;
			_nm = nm;
			_azureMS = azureMS;

			sm.FindNode += Sm_Findnode;
		}

		/// <summary>
		/// Browse server's data tree
		/// </summary>
		public TreeNode BrowseTree()
		{
			if (m_session is null)
			{
				m_session = OpcConfiguration.OpcUaClientSession;
			}

			m_session.Browse(null, null, ObjectIds.ObjectsFolder, 0u, BrowseDirection.Forward,
									ReferenceTypeIds.HierarchicalReferences, true, (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method, out byte[] nextCp, out ReferenceDescriptionCollection nextRefs);

			int deepLvl = 0;
			string parentName = "ROOT";

			var treeNode = new TreeNode()
			{
				Name = parentName,
				NodeId = deepLvl.ToString()
			};


			if (!Extensions.ReadSettings().CreateFullTree)
				return treeNode;

			Console.WriteLine("DisplayName  |  BrowseName  |  NodeClass\n-------------+--------------+-----------");
			// browse all the available nodes
			foreach (var rd in nextRefs)
			{
				/// skip predefined
				if ((rd.DisplayName.Text == "Server" || rd.DisplayName.Text == "Aliases"))
					continue;

				BrowseFoldersFunc(rd, treeNode.Children);
			}

			return treeNode;

			void BrowseFoldersFunc(ReferenceDescription rd, HashSet<TreeNode> treeNodeList)
			{
				deepLvl++;
				string currentName = rd.DisplayName.Text;
				string currentNodeId = rd.NodeId.Identifier.ToString();
				Console.WriteLine($"[{deepLvl} - {parentName}] {rd.DisplayName}  |   {rd.BrowseName}  |   {rd.NodeClass}   |   {rd.NodeId}");

				/// skip if already found
				var valueExist = treeNodeList.FirstOrDefault(c => c.NodeId == currentNodeId);
				if (valueExist != null)
					return;

				/// add same row children
				treeNodeList.Add(new()
				{
					NodeId = currentNodeId,
					Name = currentName,
				});

				m_session.Browse(null, null, ExpandedNodeId.ToNodeId(rd.NodeId, m_session.NamespaceUris), 0u, BrowseDirection.Forward,
										ReferenceTypeIds.HierarchicalReferences, true, (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method, out byte[] nextCp, out ReferenceDescriptionCollection nextRefs);

				/// next row children
				if (nextRefs != null && nextRefs.Count > 0)
				{
					foreach (var item in nextRefs)
					{
						parentName = rd.DisplayName.Text;
						var currentNode = treeNodeList.FirstOrDefault(c => c.NodeId == rd.NodeId.Identifier.ToString());
						BrowseFoldersFunc(item, currentNode.Children);
					}
				}
				deepLvl--;
			}
		}

		/// <summary>
		/// Reads server's nodes and prepares tree
		/// </summary>
		/// <param name="itemsToFind"></param>
		/// <param name="newAppSettings">if signalR have updated appsettings it sends it here</param>
		/// <returns></returns>
		public async Task<TreeNode> FindNodesRecursively(string groupName = "All")
		{
			int deepLvl = -1;
			string parentName = "ROOT";
			var itemsToFind = new HashSet<BaseNode>(await _repo.GetConfigNodes(groupName).ToListAsync());

			if (itemsToFind is null || itemsToFind.Count == 0)
				return null;

			TreeNode treeNode = new()
			{
				NodeId = (deepLvl + 1).ToString(),
				Name = parentName,
			};

			HashSet<BaseNode> notFoundItems = new(itemsToFind); ///scince we are checking HasNeededChild we need a copy
			//TODO: handle session
			if (m_session is null)
			{
				m_session = OpcConfiguration.OpcUaClientSession;
			}
			m_session.Browse(null, null, ObjectIds.ObjectsFolder, 0u, BrowseDirection.Forward,
			ReferenceTypeIds.HierarchicalReferences, true, (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method, out byte[] nextCp, out ReferenceDescriptionCollection nextRefs);
			Console.WriteLine();
			LoggerManager.Logger.Information("# Searching started");

			int foundCounter = 0;
			LoggerManager.Logger.Information("Found {0} nodes", foundCounter);
			int lineToUpdate = Console.CursorTop;

			Console.WriteLine("DisplayName  |  BrowseName  |  NodeClass\n-------------+--------------+-----------");
			// browse all the available nodes
			foreach (var rd in nextRefs)
			{
				/// if mssql installed and
				if ((rd.DisplayName.Text == "Server" || rd.DisplayName.Text == "Aliases"))
					continue;

				FindNodesRecursivelyFunc(rd, treeNode.Children);
			}

			foreach (var item in notFoundItems)
			{
				LoggerManager.Logger.Error("404: {0} | {1}", item.NodeId, item.Name);
			}
			return treeNode;

			void FindNodesRecursivelyFunc(ReferenceDescription rd, HashSet<TreeNode> treeNodeList)
			{
				deepLvl++;
				bool valueFound = false;
				string currentNodeId = rd.NodeId.Identifier.ToString();

				/// add same row children
				var currentNode = (new TreeNode
				{
					NodeId = currentNodeId,
					Name = rd.DisplayName.Text,
				});

				/// skip if already found
				var valueExist = treeNodeList.FirstOrDefault(c => c.NodeId == currentNodeId);
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

					var nodeDto = treeNodeList.FirstOrDefault(currentNode);
					BaseNode baseNode = new()
					{
						Name = nodeDto.Name,
						NodeId = nodeDto.NodeId,
					};
					NodeData entity = new(baseNode, _nm.ReadNodeValue(rd.NodeId));

					_repo.AddMonitoringValue(entity).Wait();

					// subscription mng
					_sm.SubscribeNew(rd);

					_azureMS.SendNodeAsync(entity).Wait();
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
						var childNode = treeNodeList.FirstOrDefault(c => c.NodeId == currentNodeId.ToString());
						FindNodesRecursivelyFunc(nextRd, childNode.Children);
					}
				}
				/// do current parent has needed child
				bool hasNeeded = HasNeededChild(treeNodeList.FirstOrDefault(c => c.NodeId == currentNodeId).Children, itemsToFind);
				if (!valueFound && !hasNeeded || nextRefs.Count == 0 && !valueFound)
				{
					treeNodeList.Remove(currentNode);
				}

				deepLvl--;
			}

			bool HasNeededChild(HashSet<TreeNode> treeNode, HashSet<BaseNode> itemsToFind = null)
			{
				foreach (var itemTree in treeNode)
				{
					if (itemsToFind.Any(c => c.NodeId == itemTree.NodeId))
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

		public static BaseNode FindNode(NodeId node)
		{
			return itemsToFind.FirstOrDefault(c => c.NodeId == node.Identifier.ToString());
		}

		public async Task WriteTree(string graphName, TreeNode treeData)
		{
			var jsonTree = JsonConvert.SerializeObject(treeData, Formatting.Indented);
			await File.WriteAllTextAsync(graphName, jsonTree);

			LoggerManager.Logger.Information(string.Format("# {0} created. {1}", graphName, Directory.GetCurrentDirectory()));
		}

		private async Task Sm_Findnode(object sender, NewNode node)
		{
			await FindNodesRecursively(node.Group);
		}
	}
}
