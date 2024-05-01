namespace QIA.Opc.Application.Responses;

using Qia.Opc.Domain.Entities;

public class SubscriptionValue
{
    public string Guid { get; set; }
    public uint OpcUaId { get; set; }
    public string DisplayName { get; set; }
    public int PublishingInterval { get; set; }
    public uint ItemsCount { get; set; }
    public uint SequenceNumber { get; set; }
    public string SessionNodeId { get; set; }
    public bool PublishingEnabled { get; set; }
    public List<MonitoredItemValue> MonitoredItems { get; set; } = new List<MonitoredItemValue>();
    public float MaxValue { get; set; }
    public float MinValue { get; set; }
}
