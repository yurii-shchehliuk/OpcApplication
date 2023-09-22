using Opc.Ua;

namespace QIA.Plugin.OpcClient.Services.Interfaces
{
	public interface ISubscriptionManager
	{
		void SubscribeNew(ReferenceDescription reference);

		event SubscriptionManager.TreeManagerHandler FindNode;
	}
}