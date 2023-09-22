using Microsoft.EntityFrameworkCore;
using QIA.Plugin.OpcClient.Core;
using QIA.Plugin.OpcClient.Database;
using QIA.Plugin.OpcClient.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace QIA.Plugin.OpcClient.Repository
{
	/// TODO: generic repository
	public class DataAccess<T> : IDataAccess<T> where T : class
	{
		private readonly OpcDbContext _context;

		public DataAccess(OpcDbContext context)
		{
			_context = context;
		}

		public async Task AddAsync(T entity)
		{
			if (!Extensions.ReadSettings().SaveToDb)
				return;

			try
			{
				_context.Set<T>().Add(entity);
				_context.SaveChanges();
				await Task.CompletedTask.WaitAsync(CancellationToken.None);
			}
			catch (Exception ex)
			{
				LoggerManager.Logger.Error("Adding error {0}", ex.Message);
				throw;
			}
		}

		public async Task AddMonitoringValue(NodeData entity)
		{
			if (!Extensions.ReadSettings().SaveToDb)
				return;

			try
			{
				_context.MonitorNodes.Add(entity);
				_context.SaveChanges();
				await Task.CompletedTask.WaitAsync(CancellationToken.None);
			}
			catch (Exception ex)
			{
				LoggerManager.Logger.Error("Adding error {0}", ex.Message);
				throw;
			}
		}

		public void UpdateAsync(T entity)
		{
			try
			{
				_context.Set<T>().Attach(entity);
				_context.SaveChanges();
			}
			catch (Exception ex)
			{
				LoggerManager.Logger.Error("Updating error {0}", ex.Message);
				throw;
			}
		}

		public async Task<NodeData> FindNodeByIdAsync(string id)
		{
			if (!Extensions.ReadSettings().SaveToDb)
				return null;
			var t = await _context.MonitorNodes.Where(c => c.NodeId == id).ToListAsync();
			return t.LastOrDefault();
		}

		public async Task<T> FindByIdAsync(T id)
		{
			if (!Extensions.ReadSettings().SaveToDb)
				return null;
			var t = await _context.Set<T>().FirstOrDefaultAsync(c => c == id);
			return t;
		}

		public async Task DeleteAsync(T entity)
		{
			_context.Set<T>().Remove(entity);
			await _context.SaveChangesAsync();
		}

		public async Task<IReadOnlyList<T>> ListAllAsync()
		{
			return await _context.Set<T>().ToListAsync();
		}

		public IQueryable<BaseNode> GetConfigNodes(string groupName)
		{
			var t = _context.NodesConfig.Where(c => c.Group == groupName);
			return t;
		}

		public async Task UpdateSettingsAsync(Group updatedGroup)
		{
			var existingGroup = await _context.Groups
					.Include(g => g.AppSettings)
							.ThenInclude(a => a.AzureEventHub)
									.ThenInclude(ae => ae.EventHubSender)
					.Include(g => g.AppSettings)
							.ThenInclude(a => a.AzureEventHub)
									.ThenInclude(ae => ae.EventHubConsumer)
					.FirstOrDefaultAsync(g => g.Id == updatedGroup.Id);

			if (existingGroup == null)
			{
				_context.Groups.Add(updatedGroup);
				_context.SaveChanges();
				return;
			}

			//_context.Entry(existingGroup).CurrentValues.SetValues(updatedGroup);

			if (updatedGroup.AppSettings != null)
			{
				_context.Entry(existingGroup.AppSettings).CurrentValues.SetValues(updatedGroup.AppSettings);

				if (updatedGroup.AppSettings.AzureEventHub != null)
				{
					_context.Entry(existingGroup.AppSettings.AzureEventHub).CurrentValues.SetValues(updatedGroup.AppSettings.AzureEventHub);

					if (updatedGroup.AppSettings.AzureEventHub.EventHubSender != null)
					{
						_context.Entry(existingGroup.AppSettings.AzureEventHub.EventHubSender).CurrentValues.SetValues(updatedGroup.AppSettings.AzureEventHub.EventHubSender);
					}

					if (updatedGroup.AppSettings.AzureEventHub.EventHubConsumer != null)
					{
						_context.Entry(existingGroup.AppSettings.AzureEventHub.EventHubConsumer).CurrentValues.SetValues(updatedGroup.AppSettings.AzureEventHub.EventHubConsumer);
					}
				}
			}

			await _context.SaveChangesAsync();
		}
	}
}
