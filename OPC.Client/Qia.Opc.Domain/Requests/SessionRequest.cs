namespace QIA.Opc.Domain.Requests
{
	public class SessionRequest
	{
		public string Name { get; set; }
		public string EndpointUrl { get; set; }
		public string? SessionGuidId { get; set; }
		public string? SessionNodeId { get; set; }
	}
}
