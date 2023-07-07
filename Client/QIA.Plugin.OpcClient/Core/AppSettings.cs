namespace QIA.Plugin.OpcClient.Core
{
    public class AppSettings
    {
        /// <summary>
        /// When static node found in nodemanager
        /// </summary>
        public bool SaveToDb { get; set; } = true;
        public bool SaveToAzure { get; set; } = false;
        public bool RecreateDb { get; set; } = false;
    }

    public class AzureEventHub
    {
        public string EndpointUrl { get; set; }
        public string HubName { get; set; }
        public EventHubSenderObj EventHubSender { get; set; }
        public EventHubConsumerObj EventHubConsumer { get; set; }

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
