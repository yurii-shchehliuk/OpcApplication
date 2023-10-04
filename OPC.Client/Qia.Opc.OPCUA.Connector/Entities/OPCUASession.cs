using Opc.Ua.Client;
using Qia.Opc.Domain.Entities.Enums;

namespace Qia.Opc.OPCUA.Connector.Entities
{
	public class OPCUASession
	{
		public string Name { get; set; }
		public string SessionId { get; set; }
		public string EndpointUrl { get; set; }
		public Session Session { get; set; }
		public SessionState State { get; set; } = SessionState.Disconnected;
		public DateTime CreatedAt { get; set; }
		public TimeSpan ExpiryDuration { get; set; } = TimeSpan.FromHours(1);
		public DateTime LastAccessed { get; set; }

	}

}
