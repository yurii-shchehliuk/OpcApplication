using Opc.Ua;

namespace QIA.Plugin.OpcClient.Services.Interfaces
{
    public interface INodeManager
    {
        string ReadNodeValue(ExpandedNodeId currentNodeId);
        void FindInCache(NodeId currentNodeId);
        void CallMethod(NodeId parrentObjectId, NodeId methodId, params object[] inputArguments);

    }
}
