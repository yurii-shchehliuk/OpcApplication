namespace QIA.Opc.Application.Requests;

public class SessionRequest
{
    public string SessionNodeId { get; set; }
    public string Guid { get; set; }
    public string Name { get; set; }
    public string EndpointUrl { get; set; }
}
