using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QIA.Plugin.OpcClient.Entities
{
    public class AzureEventHub : BaseEntity
    {
        public string EndpointUrl { get; set; }
        public string HubName { get; set; }
        public int? EventHubSenderId { get; set; }
        public virtual EventHubSenderObj EventHubSender { get; set; }
        public int? EventHubConsumerId { get; set; }
        public virtual EventHubConsumerObj EventHubConsumer { get; set; }
        [JsonIgnore]
        public virtual AppSettings AppSettings { get; set; }

        public class EventHubSenderObj : BaseEntity
        {
            public string PrimaryKey { get; set; }
            public string PolicyName { get; set; }
            [JsonIgnore]
            public virtual AzureEventHub AzureEventHub { get; set; }
        }

        public class EventHubConsumerObj : BaseEntity
        {
            public string ConntainerName { get; set; }
            public string BlobConnString { get; set; }
            public string ConsumerGroup { get; set; }
            public string PrimaryKey { get; set; }
            public string PolicyName { get; set; }
            [JsonIgnore]
            public virtual AzureEventHub AzureEventHub { get; set; }
        }
    }
}
