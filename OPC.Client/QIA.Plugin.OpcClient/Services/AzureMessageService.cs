using Azure;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Processor;
using Azure.Messaging.EventHubs.Producer;
using Azure.Storage.Blobs;
using QIA.Plugin.OpcClient.Core;
using QIA.Plugin.OpcClient.Entities;
using QIA.Plugin.OpcClient.Services.Interfaces;
using System;
using System.Text;
using System.Threading.Tasks;

namespace QIA.Plugin.OpcClient.Services
{
    public class AzureMessageService : IAzureMessageService
    {
        private readonly EventHubProducerClient producerClient;
        private readonly AzureEventHub azureEventHub;

        public AzureMessageService()
        {
            azureEventHub = Extensions.ReadSettings().AzureEventHub;

            ///https://learn.microsoft.com/en-us/azure/event-hubs/connect-event-hub
            producerClient = new EventHubProducerClient(azureEventHub.EndpointUrl, azureEventHub.HubName,
               new AzureNamedKeyCredential(azureEventHub.EventHubSender.PolicyName, azureEventHub.EventHubSender.PrimaryKey));
        }

        public async Task SendNodeAsync(NodeData message)
        {
            try
            {
                //TODO: split with services and repos interfaces
                if (!Extensions.ReadSettings().SaveToAzure)
                {
                    return;
                }

                LoggerManager.Logger.Information(string.Format("EventHub > Sending node: {0} {1}", message.Name, message.NodeId));

                using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

                if (!eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes($"Event {message}"))))
                {
                    // if it is too large for the batch
                    throw new Exception($"EventHub : Event {message} is too large for the batch and cannot be sent.");
                }

                await producerClient.SendAsync(eventBatch);
                LoggerManager.Logger.Information(string.Format("EventHub  < Message send"));
            }
            catch (Exception ex)
            {
                LoggerManager.Logger.Error(ex.Message, ex);
                throw;
            }
        }

        public async Task ReadMessageAsync()
        {
            try
            {
                //TODO:
                var storageClient = new BlobContainerClient(azureEventHub.EventHubConsumer.BlobConnString, azureEventHub.EventHubConsumer.ConntainerName);

                var proc = new EventProcessorClient(storageClient, azureEventHub.EventHubConsumer.ConsumerGroup, azureEventHub.EndpointUrl, azureEventHub.HubName, new AzureNamedKeyCredential(azureEventHub.EventHubConsumer.PolicyName, azureEventHub.EventHubConsumer.PrimaryKey));

                proc.ProcessEventAsync += ProcessEventHandler;
                proc.ProcessErrorAsync += ProcessErrorHandler;

                await proc.StartProcessingAsync();
                LoggerManager.Logger.Information("# Azure listening started");
            }
            catch (Exception ex)
            {
                LoggerManager.Logger.Error(ex.Message);
                throw;
            }

            Task ProcessEventHandler(ProcessEventArgs eventArgs)
            {
                // Write the body of the event to the console window
                var result = ByteArrayToObject(eventArgs.Data.Body.ToArray());
                Console.WriteLine("\tReceived event: {0}", result);

                return Task.CompletedTask;
            }

            Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
            {
                // Write details about the error to the console window
                Console.WriteLine($"\tPartition '{eventArgs.PartitionId}': an unhandled exception was encountered. This was not expected to happen.");
                LoggerManager.Logger.Error(eventArgs.Exception, eventArgs.Exception.Message, eventArgs.Exception.InnerException.Message);

                return Task.CompletedTask;
            }

            string ByteArrayToObject(byte[] arrBytes)
            {
                return Encoding.UTF8.GetString(arrBytes);
            }
        }
    }
}
