namespace QIA.Opc.Application.Responses;

using Qia.Opc.Domain.Entities.Enums;

public class SessionResponse
{
    public string Guid { get; set; }
    public string SessionNodeId { get; set; }
    public string Name { get; set; }
    public string EndpointUrl { get; set; }
    public SessionState State { get; set; } = SessionState.Disconnected;
}
