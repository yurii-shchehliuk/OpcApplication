namespace Qia.Opc.Domain.Entities;

using System.ComponentModel.DataAnnotations;
using Qia.Opc.Domain.Entities.Interfaces;

/// <summary>
/// node reference 
/// </summary>
public class MonitoredItemConfig : IBaseEntity
{
    [Key]
    public string Guid { get; set; } = System.Guid.NewGuid().ToString();
    public int OpcUaId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    public string DisplayName { get; set; }
    public string StartNodeId { get; set; }
    public int SamplingInterval { get; set; }
    public uint QueueSize { get; set; }
    public bool DiscardOldest { get; set; }
    public string IndexRange { get; set; }

    //
    public string SubscriptionGuid { get; set; }
}
