namespace QIA.Opc.Infrastructure.Entities;

using global::Opc.Ua.Client;

public class OPCUASubscription
{
    public string Guid { get; set; }
    public Subscription Subscription { get; set; }
}
