using QIA.Plugin.OpcClient.Core;
using QIA.Plugin.OpcClient.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QIA.Plugin.OpcClient.Repository
{
    public interface IDataAccess
    {
        Task AddAsync(NodeData entity);
        Task DeleteAsync(NodeData entity);
        Task<NodeData> FindByIdAsync(string id);
        Task<IReadOnlyList<NodeData>> ListAllAsync();
        Task UpdateAsync(NodeData entity);

    }
}