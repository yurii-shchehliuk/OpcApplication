using QIA.Plugin.OpcClient.Entities;
using System.Threading.Tasks;

namespace QIA.Plugin.OpcClient.Services.Interfaces
{
    public interface IAzureMessageService
    {
        Task ReadMessageAsync();
        Task SendMessageAsync(NodeData message);
    }
}