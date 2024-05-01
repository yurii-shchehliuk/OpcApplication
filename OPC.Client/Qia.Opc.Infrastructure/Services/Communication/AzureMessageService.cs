namespace QIA.Opc.Infrastructure.Services.Communication;

using Azure;
using Azure.Messaging.EventHubs.Producer;
using Qia.Opc.Domain.Entities;
using QIA.Opc.Application.Settings;
using QIA.Opc.Infrastructure.Application;

public class AzureMessageService : IDisposable, IAsyncDisposable
{
    private readonly EventHubProducerClient _producerClient;
    private readonly AzureEventHub _azureEventHub;

    public AzureMessageService(IAppSettings appSettings)
    {
        _azureEventHub = appSettings.AzureEventHub;

        ///https://learn.microsoft.com/en-us/azure/event-hubs/connect-event-hub
        _producerClient = new EventHubProducerClient(_azureEventHub.EndpointUrl, _azureEventHub.HubName,
             new AzureNamedKeyCredential(_azureEventHub.EventHubSender.PolicyName, _azureEventHub.EventHubSender.PrimaryKey));
    }

    public async Task SendNodeAsync(MonitoredItemValue message)
    {
        try
        {
            LoggerManager.Logger.Information(string.Format("EventHub > Sending node: {0} {1}", message.DisplayName, message.StartNodeId));

            using EventDataBatch eventBatch = await _producerClient.CreateBatchAsync();


            await _producerClient.SendAsync(eventBatch);
            LoggerManager.Logger.Information(string.Format("EventHub  < Message send"));
        }
        catch (Exception ex)
        {
            LoggerManager.Logger.Error(ex.Message, ex);
            throw;
        }
    }
	public async ValueTask DisposeAsync()
	{
		if (_producerClient != null)
		{
			await _producerClient.DisposeAsync();
		}
	}

	public void Dispose() => DisposeAsync().AsTask().GetAwaiter().GetResult();
}
