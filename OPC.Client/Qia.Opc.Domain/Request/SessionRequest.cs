namespace QIA.Opc.Domain.Request
{
    public class SessionRequest
    {
        public string Name { get; set; }
        public string EndpointUrl { get; set; }
        public string? SessionId { get; set; } = "";
    }
}
