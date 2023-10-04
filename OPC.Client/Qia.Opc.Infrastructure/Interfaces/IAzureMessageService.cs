using Qia.Opc.Domain.Entities;

namespace Qia.Opc.Infrastrucutre.Interfaces
{
	public interface IAzureMessageService
	{
		Task ReadMessageAsync();
		Task SendNodeAsync(NodeData message);
	}
}