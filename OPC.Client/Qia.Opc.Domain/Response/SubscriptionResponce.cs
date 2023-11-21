namespace QIA.Opc.Domain.Response
{
    public class SubscriptionResponce
    {
        public uint Id { get; set; }
        public string DisplayName { get; set; }
        public int PublishInterval { get; set; }
        public uint ItemsCount { get; set; }
        public uint SequenceNumber { get; set; }
        public string SessionNodeId { get; set; }
        public bool PublishingEnabled { get; set; }
		public List<MonitoredItemResponse> MonitoringItems { get; set; } = new List<MonitoredItemResponse>();
	}
}
