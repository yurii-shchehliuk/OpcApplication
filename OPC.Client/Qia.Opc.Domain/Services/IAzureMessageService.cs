using Qia.Opc.Domain.Entities;

namespace QIA.Opc.Domain.Services
{
	public interface IAzureMessageService
	{
		Task ReadMessageAsync();
		Task SendNodeAsync(MonitoredItemValue message);
	}
}