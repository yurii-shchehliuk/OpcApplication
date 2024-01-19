namespace QIA.Opc.Domain.Requests
{
	public class SessionRequest
	{
		public string? Guid { get; set; }
		public string Name { get; set; }
		public string EndpointUrl { get; set; }
		public string? SessionNodeId { get; set; }
	}
}
