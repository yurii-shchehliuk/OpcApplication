namespace QIA.Opc.Application.Requests;

public class SubscriptionRequest
{
    public string Guid { get; set; } = "";
    public uint OpcUaId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public float MaxValue { get; set; }
    public float MinValue { get; set; }
    public int PublishingInterval { get; set; } = 1000;
    public uint KeepAliveCount { get; set; } = 10;
    public uint LifetimeCount { get; set; } = 1000;
    public uint MaxNotificationsPerPublish { get; set; }
    public byte Priority { get; set; } = 255;
    public bool PublishingEnabled { get; set; }
}
