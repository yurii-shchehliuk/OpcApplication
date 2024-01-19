using Qia.Opc.Domain.Entities.Enums;

namespace QIA.Opc.Domain.Responses
{
	public class SessionResponse
	{
		public string Guid { get; set; }
		public string? SessionNodeId { get; set; }
		public string Name { get; set; }
		public string EndpointUrl { get; set; }
		public SessionState State { get; set; } = SessionState.Disconnected;
	}
}
