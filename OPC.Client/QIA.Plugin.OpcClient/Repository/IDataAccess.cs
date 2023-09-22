using QIA.Plugin.OpcClient.Core;
using QIA.Plugin.OpcClient.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QIA.Plugin.OpcClient.Repository
{
	public interface IDataAccess<T> where T : class
	{
		Task AddAsync(T entity);
		Task DeleteAsync(T entity);
		Task<T> FindByIdAsync(T id);
		Task<IReadOnlyList<T>> ListAllAsync();
		void UpdateAsync(T entity);

		Task AddMonitoringValue(NodeData entity);
		Task<NodeData> FindNodeByIdAsync(string id);
		Task UpdateSettingsAsync(Group entity);
		IQueryable<BaseNode> GetConfigNodes(string groupName);

	}
}