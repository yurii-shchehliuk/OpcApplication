namespace QIA.Opc.Infrastructure.Extensions;

using global::Opc.Ua;

public static class NodeIdExtensions
{
    public static bool TryParseNodeId(this string nodeIdString, out NodeId nodeId)
    {
        try
        {
            if (string.IsNullOrEmpty(nodeIdString))
            {
                nodeId = null;
                return false;
            }
            nodeId = new NodeId(nodeIdString);
            return true;
        }
        catch
        {
            nodeId = null;
            return false;
        }
    }
}
