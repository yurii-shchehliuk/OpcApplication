using QIA.Plugin.OpcClient.Core;
using QIA.Plugin.OpcClient.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QIA.Plugin.OpcClient.Repository
{
    public interface IDataAccess
    {
        Task AddAsync(SampleNode entity);
        Task DeleteAsync(SampleNode entity);
        Task<SampleNode> FindByIdAsync(string id);
        Task<IReadOnlyList<SampleNode>> ListAllAsync();
        AppConfig GetAppConfig();
        Task UpdateAsync(SampleNode entity);

    }
}