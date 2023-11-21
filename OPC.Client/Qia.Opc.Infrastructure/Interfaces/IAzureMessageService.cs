using Qia.Opc.Domain.Entities;

namespace QIA.Opc.Infrastructure.Interfaces
{
	public interface IAzureMessageService
	{
		Task ReadMessageAsync();
		Task SendNodeAsync(NodeValue message);
	}
}