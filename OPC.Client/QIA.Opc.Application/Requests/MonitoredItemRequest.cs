namespace QIA.Opc.Application.Requests;

public class MonitoredItemRequest
{
    public string DisplayName { get; set; } = string.Empty;
    public string StartNodeId { get; set; } = string.Empty;
    public int SamplingInterval { get; set; }
    public uint QueueSize { get; set; }
    public bool DiscardOldest { get; set; }
}
