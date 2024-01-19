namespace QIA.Opc.Domain.Requests
{
	public class SubscriptionRequest
	{
		public string? Guid { get; set; }
		public uint? OpcUaId { get; set; } = 0;
		public string DisplayName { get; set; } = string.Empty;
		public int MaxValue { get; set; } = 0;
		public int MinValue { get; set; } = 0;
		public int PublishingInterval { get; set; } = 1000;
		public uint KeepAliveCount { get; set; } = 10;
		public uint LifetimeCount { get; set; } = 1000;
		public uint MaxNotificationsPerPublish { get; set; } = 0;
		public byte Priority { get; set; } = 255;
		public bool PublishingEnabled { get; set; } = false;
	}
}
