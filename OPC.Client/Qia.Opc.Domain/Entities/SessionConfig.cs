namespace Qia.Opc.Domain.Entities;

using Qia.Opc.Domain.Entities.Interfaces;
using QIA.Opc.Domain.Entities;

public class SessionConfig : IBaseEntity
{
    public string Guid { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    public string Name { get; set; }
    public string EndpointUrl { get; set; }
    public List<SubscriptionConfig> SubscriptionConfigs { get; set; } = new();
}
