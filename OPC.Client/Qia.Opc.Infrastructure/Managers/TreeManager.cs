namespace QIA.Opc.Infrastructure.Managers;

using global::Opc.Ua;
using global::Opc.Ua.Client;
using Qia.Opc.Domain.Entities;
using QIA.Opc.Infrastructure.Application;
using QIA.Opc.Infrastructure.Extensions;

public class TreeManager
{
    private readonly SessionManager _sessionManager;

    public TreeManager(SessionManager sessionManager)
    {
        _sessionManager = sessionManager;
    }

    /// <summary>
    /// Browse server's data tree
    /// </summary>
    public async Task<TreeNode> BrowseTreeAsync(string sessionGuid)
    {
        _sessionManager.TryGetSession(sessionGuid, out Entities.OPCUASession session);

        LoggerManager.Logger.Information($"Parsing full tree started for {session.Name}");
        session.Session.Browse(null, null, ObjectIds.ObjectsFolder, 0u, BrowseDirection.Forward,
                        ReferenceTypeIds.HierarchicalReferences, true,
                        (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method,
                        out _, out ReferenceDescriptionCollection nextRefs);

        TreeNode rootNode = new()
        {
            DisplayName = "ROOT",
            StartNodeId = "0",
        };

        await ProcessNodesAsync(nextRefs, rootNode, session.Session, 1);

        LoggerManager.Logger.Information($"Parsing full tree compleated for {session.Name}");
        return rootNode;
    }

    private static async Task ProcessNodesAsync(ReferenceDescriptionCollection nodes, TreeNode parent, Session session, int level)
    {
        foreach (ReferenceDescription node in nodes)
        {
            var nodeId = node.NodeId.Identifier.ToString();
            if (parent.Children.Any(c => c.StartNodeId == nodeId))
            {
                continue;
            }

            TreeNode treeNode = new()
            {
                DisplayName = node.DisplayName.Text,
                StartNodeId = nodeId,
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

    public TreeNode BrowseChildren(string sessionGuid, TreeNode treeNode)
    {
        _sessionManager.TryGetSession(sessionGuid, out Entities.OPCUASession session);
        Browser browser = new(session.Session)
        {
            BrowseDirection = BrowseDirection.Forward,
            ReferenceTypeId = ReferenceTypeIds.HierarchicalReferences,
            IncludeSubtypes = true,
            NodeClassMask = 0,
            ContinueUntilDone = false,
        };

        ReferenceDescription reference = null;
        if (treeNode.StartNodeId.TryParseNodeId(out NodeId nodeId))
        {
            //INode node = browser.Session.NodeCache.Find(nodeId);
            reference = new ReferenceDescription()
            {
                NodeId = nodeId,
                DisplayName = treeNode.DisplayName,
                IsForward = true,
                ReferenceTypeId = ReferenceTypeIds.References,
                TypeDefinition = null
            };
        }

        // fetch references.
        ReferenceDescriptionCollection references;
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

    private static void AddReferences(TreeNode treeNode, ReferenceDescriptionCollection references)
    {
        treeNode.Children = new HashSet<TreeNode>();
        foreach (ReferenceDescription item in references)
        {
            TreeNode treeItem = new()
            {
                DisplayName = item.DisplayName.ToString(),
                StartNodeId = item.NodeId.ToString(),
            };

            if (!treeNode.Children.Any(c => c.StartNodeId == item.NodeId.ToString()))
            {
                treeNode.Children.Add(treeItem);
            }
        }
    }
}
