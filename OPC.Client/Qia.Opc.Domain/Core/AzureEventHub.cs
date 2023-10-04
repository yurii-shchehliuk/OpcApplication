namespace Qia.Opc.Domain.Core
{
	public class AzureEventHub
	{
		public string EndpointUrl { get; set; }
		public string HubName { get; set; }
		public int? EventHubSenderId { get; set; }
		public virtual EventHubSenderObj EventHubSender { get; set; }
		public int? EventHubConsumerId { get; set; }
		public virtual EventHubConsumerObj EventHubConsumer { get; set; }

		public class EventHubSenderObj
		{
			public string PrimaryKey { get; set; }
			public string PolicyName { get; set; }

		}

		public class EventHubConsumerObj
		{
			public string ConntainerName { get; set; }
			public string BlobConnString { get; set; }
			public string ConsumerGroup { get; set; }
			public string PrimaryKey { get; set; }
			public string PolicyName { get; set; }

		}
	}
}
