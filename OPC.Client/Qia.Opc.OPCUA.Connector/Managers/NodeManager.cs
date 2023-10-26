using Opc.Ua;

namespace Qia.Opc.OPCUA.Connector.Managers
{
	public class NodeManager
	{
		private readonly SessionManager sessionManager;

		public NodeManager(SessionManager sessionManager)
		{
			this.sessionManager = sessionManager;
		}

		public Node FindNodeOnServer(string nodeId)
		{

			NodeId nodeIdToSearch = new NodeId(nodeId);

			var session = sessionManager.CurrentSession.Session;
			Node node = session.ReadNode(nodeIdToSearch);

			return node;
		}

		public DataValue ReadValue(string nodeId)
		{
			var session = sessionManager.CurrentSession.Session;
			try
			{
				DataValue nodeValue = session.ReadValue(nodeId);
				return nodeValue;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}
}
