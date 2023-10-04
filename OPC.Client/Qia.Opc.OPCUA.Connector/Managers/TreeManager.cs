using AutoMapper;
using Opc.Ua;
using Opc.Ua.Client;
using Qia.Opc.Domain.Common;
using Qia.Opc.Domain.Core;
using Qia.Opc.Domain.Entities;
using static System.Net.Mime.MediaTypeNames;

namespace Qia.Opc.OPCUA.Connector.Managers
{

	public class TreeManager
	{
		private readonly SessionManager sessionManager;
		private readonly IMapper mapper;
		// events
		public delegate Task TreeServiceHandler(object sender, Domain.Entities.NodeReferenceEntity node);
		public event TreeServiceHandler NodeFound;

		public TreeManager(SessionManager sessionManager, IMapper mapper)
		{
			this.sessionManager = sessionManager;
			this.mapper = mapper;
		}

		/// <summary>
		/// Browse server's data tree
		/// </summary>
		public TreeNode BrowseTree()
		{
			var session = sessionManager.CurrentSession.Session;
			session.Browse(null, null, ObjectIds.ObjectsFolder, 0u, BrowseDirection.Forward,
									ReferenceTypeIds.HierarchicalReferences, true,
									(uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method,
									out byte[] nextCp, out ReferenceDescriptionCollection nextRefs);

			int deepLvl = 0;
			string parentName = "ROOT";

			var treeNode = new TreeNode()
			{
				DisplayName = parentName,
				NodeId = deepLvl.ToString(),
			};
			;
			Console.WriteLine("DisplayName  |  BrowseName  |  NodeClass\n" +
							  "-------------+--------------+-----------");
			// browse all the available nodes
			foreach (var rd in nextRefs)
			{
				BrowseFoldersFunc(rd, treeNode.Children);
			}

			return treeNode;

			void BrowseFoldersFunc(ReferenceDescription rd, HashSet<TreeNode> treeNodeList)
			{
				deepLvl++;
				string currentName = rd.DisplayName.Text;
				string currentNodeId = rd.NodeId.Identifier.ToString();
				Console.WriteLine($"[{deepLvl} - {parentName}] {rd.DisplayName}  |   {rd.BrowseName}  |   {rd.NodeClass}   |   {rd.NodeId}");

				/// skip if already added
				var valueExist = treeNodeList.FirstOrDefault(c => c.NodeId == currentNodeId);
				if (valueExist != null)
					return;

				/// same row children
				treeNodeList.Add(new()
				{
					NodeId = currentNodeId,
					DisplayName = currentName,
				});

				session.Browse(null, null, ExpandedNodeId.ToNodeId(rd.NodeId, session.NamespaceUris), 0u, BrowseDirection.Forward,
										ReferenceTypeIds.HierarchicalReferences, true,
										(uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method, out byte[] nextCp,
										out ReferenceDescriptionCollection nextRefs);

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
		public TreeNode FindNodesRecursively(HashSet<Domain.Entities.NodeReferenceEntity> itemsToFind)
		{
			if (itemsToFind is null || itemsToFind.Count() == 0)
				return null;

			int deepLvl = 0;
			string parentName = "ROOT";
			TreeNode treeGraph = new()
			{
				NodeId = (deepLvl + 1).ToString(),
				DisplayName = parentName,
			};

			HashSet<Domain.Entities.NodeReferenceEntity> notFoundItems = new(itemsToFind); ///scince we are checking HasNeededChild we need a copy

			var session = sessionManager.CurrentSession.Session;

			session.Browse(null, null, ObjectIds.ObjectsFolder, 0u, BrowseDirection.Forward,
					ReferenceTypeIds.HierarchicalReferences, true,
					(uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method,
					out byte[] nextCp, out ReferenceDescriptionCollection nextRefs);

			Console.WriteLine();
			LoggerManager.Logger.Information("# Searching started");

			int foundCounter = 0;
			LoggerManager.Logger.Information("Found {0} nodes", foundCounter);
			int lineToUpdate = 0;
			try
			{
				lineToUpdate = Console.CursorTop;
			}
			catch
			{
			}

			Console.WriteLine("DisplayName  |  BrowseName  |  NodeClass\n" +
							  "-------------+--------------+-----------");

			// browse all the nodes
			foreach (var rd in nextRefs)
			{
				FindNodesRecursivelyFunc(rd, treeGraph.Children);
			}

			foreach (var item in notFoundItems)
			{
				LoggerManager.Logger.Error("404: {0} | {1}", item.NodeId, item.DisplayName);
			}

			return treeGraph;

			void FindNodesRecursivelyFunc(ReferenceDescription rd, HashSet<TreeNode> treeNodeList)
			{
				deepLvl++;
				bool valueFound = false;
				string currentNodeId = rd.NodeId.ToString();

				/// add same row children
				var currentNode = (new TreeNode
				{
					NodeId = currentNodeId,
					DisplayName = rd.DisplayName.Text,
				});

				/// skip if already added
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

					var baseNode = mapper.Map<Domain.Entities.NodeReferenceEntity>(rd);
					NodeFound.Invoke(this, baseNode);

					// new tree to be populated
					valueFound = true;
					notFoundItems.Remove(foundItem);
				}

				session.Browse(null, null, ExpandedNodeId.ToNodeId(rd.NodeId, session.NamespaceUris), 0u, BrowseDirection.Forward,
										ReferenceTypeIds.HierarchicalReferences, true,
										(uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method,
										out byte[] nextCp, out ReferenceDescriptionCollection nextRefs);

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

			bool HasNeededChild(HashSet<TreeNode> treeNode, HashSet<Domain.Entities.NodeReferenceEntity> itemsToFind = null)
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

		public TreeNode BrowseChildren(TreeNode treeNode)
		{
			var session = sessionManager.CurrentSession.Session;
			Browser browser = new Browser(session)
			{
				BrowseDirection = BrowseDirection.Forward,
				ReferenceTypeId = ReferenceTypeIds.HierarchicalReferences,
				IncludeSubtypes = true,
				NodeClassMask = 0,
				ContinueUntilDone = false,
			};

			ReferenceDescription reference = null;
			if (treeNode.NodeId.TryParseNodeId(out NodeId nodeId))
			{
                INode node = browser.Session.NodeCache.Find(nodeId);
				reference = new ReferenceDescription()
				{
					NodeId = nodeId,
					BrowseName = treeNode.BrowseName,
					DisplayName = treeNode.DisplayName,
					NodeClass = treeNode.NodeClass,
					IsForward = true,
					ReferenceTypeId = ReferenceTypeIds.References,
					TypeDefinition = null
				};
			}

			// fetch references.
			ReferenceDescriptionCollection references = null;

			if (reference != null)
			{
				references = browser.Browse((NodeId)reference.NodeId);
			}
			else
			{
				references = browser.Browse(Objects.RootFolder);
			}

			// add nodes to tree.
			AddReferences(treeNode, references);

			return treeNode;
		}

		private void AddReferences(TreeNode treeNode, ReferenceDescriptionCollection references)
		{
			foreach (var item in references)
			{
				var treeItem = new TreeNode()
				{
					DisplayName = item.DisplayName.ToString(),
					NodeId = item.NodeId.ToString(),
					BrowseName = item.BrowseName.ToString(),
					NodeClass = item.NodeClass,
				};

				treeNode.Children.Add(treeItem);
			}
		}
	}
}
