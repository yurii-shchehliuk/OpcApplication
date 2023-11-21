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
		public event TreeServiceHandler NodeFound; //deprecated

		public TreeManager(SessionManager sessionManager, IMapper mapper)
		{
			this.sessionManager = sessionManager;
			this.mapper = mapper;
		}

		/// <summary>
		/// Browse server's data tree
		/// </summary>
		public async Task<TreeNode> BrowseTreeAsync()
		{
			var session = sessionManager.CurrentSession.Session;
			LoggerManager.Logger.Information($"Parsing full tree started for {sessionManager.CurrentSession.Name}");
			session.Browse(null, null, ObjectIds.ObjectsFolder, 0u, BrowseDirection.Forward,
							ReferenceTypeIds.HierarchicalReferences, true,
							(uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method,
							out _, out ReferenceDescriptionCollection nextRefs);

			var rootNode = new TreeNode()
			{
				DisplayName = "ROOT",
				NodeId = "0",
			};

			await ProcessNodesAsync(nextRefs, rootNode, session, 1);

			LoggerManager.Logger.Information($"Parsing full tree compleated for {sessionManager.CurrentSession.Name}");
			return rootNode;
		}

		private async Task ProcessNodesAsync(ReferenceDescriptionCollection nodes, TreeNode parent, Session session, int level)
		{
			foreach (var node in nodes)
			{
				string nodeId = node.NodeId.Identifier.ToString();
				if (parent.Children.Any(c => c.NodeId == nodeId))
				{
					continue;
				}

				var treeNode = new TreeNode()
				{
					DisplayName = node.DisplayName.Text,
					NodeId = nodeId,
				};

				parent.Children.Add(treeNode);

				session.Browse(null, null, ExpandedNodeId.ToNodeId(node.NodeId, session.NamespaceUris), 0u, BrowseDirection.Forward,
								ReferenceTypeIds.HierarchicalReferences, true,
								(uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method,
								out _, out ReferenceDescriptionCollection childNodes);

				if (childNodes != null && childNodes.Count > 0)
				{
					await ProcessNodesAsync(childNodes, treeNode, session, level + 1);
				}
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
				//INode node = browser.Session.NodeCache.Find(nodeId);
				reference = new ReferenceDescription()
				{
					NodeId = nodeId,
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
			treeNode.Children = new HashSet<TreeNode>();
			foreach (var item in references)
			{
				var treeItem = new TreeNode()
				{
					DisplayName = item.DisplayName.ToString(),
					NodeId = item.NodeId.ToString(),
					NodeClass = item.NodeClass,
				};

				if (!treeNode.Children.Any(c => c.NodeId == item.NodeId.ToString()))
				{
					treeNode.Children.Add(treeItem);
				}
			}
		}
	}
}
