using Opc.Ua;
using Opc.Ua.Client;
using QIA.Plugin.OpcClient.Core;
using QIA.Plugin.OpcClient.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace QIA.Plugin.OpcClient.Services
{
    public class NodeManager : INodeManager
    {
        private Session m_session;
        private NamespaceTable _namespaceTable = null;

        public NodeManager()
        {
            m_session = OpcConfiguration.OpcUaClientSession;
        }

        public void GetNamespaces()
        {
            DataValue namespaceArrayNodeValue = m_session.ReadValue(VariableIds.Server_NamespaceArray);
            _namespaceTable.Update(namespaceArrayNodeValue.GetValue<string[]>(null));
            _namespaceTable.Append("http://opcfoundation.org/QiagenOpc");

        }

        public NodeId GetNodeIdFromId(string id)
        {
            NodeId nodeId = null;
            ExpandedNodeId expandedNodeId = null;
            try
            {
                if (id.Contains("nsu="))
                {
                    expandedNodeId = ExpandedNodeId.Parse(id);
                    GetNamespaces();
                    nodeId = new NodeId(expandedNodeId.Identifier, (ushort)_namespaceTable.GetIndex(expandedNodeId.NamespaceUri));

                }
                else
                {
                    nodeId = NodeId.Parse(id);
                }
            }
            catch (Exception e)
            {
                LoggerManager.Logger.Error(e, $"The NodeId has an invalid format '{id}'!");
            }
            return nodeId;
        }


        public Node ReadNode(NodeId OpcUaNodeId)
        {
            try
            {
                Node node = OpcConfiguration.OpcUaClientSession.ReadNode(OpcUaNodeId);
                return node;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string ReadNodeValue(ExpandedNodeId currentNodeId)
        {
            try
            {
                string value = OpcConfiguration.OpcUaClientSession.ReadValue((NodeId)currentNodeId).ToString();
                //Console.WriteLine($"[node] Value of {currentNodeId} : {value}");
                return value;
            }
            catch (ServiceResultException)
            {
                /// session expired, do nothing
            }
            catch (Exception ex)
            {
                /// element has no readable value
                throw new Exception("No readable value", ex.InnerException);
            }

            return null;
        }

        public void FindInCache(NodeId currentNodeId)
        {
            if (m_session == null)
            {
                m_session = OpcConfiguration.OpcUaClientSession;
            }
            ILocalNode node = m_session.NodeCache.Find(currentNodeId) as ILocalNode;
            uint[] attributesIds = Attributes.GetIdentifiers();
            for (int ii = 0; ii < attributesIds.Length; ii++)
            {
                uint attributesId = attributesIds[ii];

                if (!node.SupportsAttribute(attributesId))
                {
                    continue;
                }

                ItemInfo info = new ItemInfo();

                info.NodeId = node.NodeId;
                info.AttributeId = attributesId;
                info.Name = Attributes.GetBrowseName(attributesId);
                info.Value = new DataValue(StatusCodes.BadWaitingForInitialData);

                ServiceResult result = node.Read(null, attributesId, info.Value);

                if (ServiceResult.IsBad(result))
                {
                    info.Value = new DataValue(result.StatusCode);
                }

                //AddItem(info);
            }

            IList<IReference> references = node.References.Find(ReferenceTypes.HasProperty, false, true, m_session.TypeTree);

            for (int ii = 0; ii < references.Count; ii++)
            {
                IReference reference = references[ii];

                ILocalNode property = m_session.NodeCache.Find(reference.TargetId) as ILocalNode;

                if (property == null)
                {
                    return;
                }

                ItemInfo info = new ItemInfo();

                info.NodeId = property.NodeId;
                info.AttributeId = Attributes.Value;
                info.Name = Utils.Format("{0}", property.DisplayName);
                info.Value = new DataValue(StatusCodes.BadWaitingForInitialData);

                ServiceResult result = property.Read(null, Attributes.Value, info.Value);

                if (ServiceResult.IsBad(result))
                {
                    info.Value = new DataValue(result.StatusCode);
                }

                //AddItem(info);
                Console.WriteLine(info.Value.Value?.ToString());
            }

            //UpdateValues();
        }

        public void CallMethod(NodeId parrentObjectId, NodeId methodId, params object[] inputArguments)
        {
            var methodToCall = new CallMethodRequest
            {
                ObjectId = parrentObjectId,
                MethodId = methodId,
                InputArguments = new VariantCollection()
            };

            var response = m_session.Call(null, new CallMethodRequestCollection { methodToCall }, out CallMethodResultCollection resColl, out DiagnosticInfoCollection infoColl);

            foreach (var result in resColl)
            {
                if (result.StatusCode != StatusCodes.Good)
                {
                    LoggerManager.Logger.Information($"Method call failed: {result.StatusCode}");
                }
                else
                {
                    LoggerManager.Logger.Information("Method call succeeded.");
                }
            }
        }

        internal class ItemInfo
        {
            public NodeId NodeId;
            public uint AttributeId;
            public string Name;
            public DataValue Value;
        }
    }
}
