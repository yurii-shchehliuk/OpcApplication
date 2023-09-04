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
    public class DataAccess : IDataAccess
    {
        private readonly OpcDbContext _context;

        public DataAccess(OpcDbContext context)
        {
            _context = context;
        }

        Guid lastNodeId = Guid.Empty;

        public async Task AddAsync(NodeData entity)
        {
            try
            {
                if (entity.Value == null || lastNodeId == entity.Id)
                {
                    return;
                }
                lastNodeId = entity.Id;
                _context.SampleNodes.Add(entity);
                _context.SaveChanges();
                await Task.CompletedTask.WaitAsync(CancellationToken.None);
            }
            catch (Exception ex)
            {
                LoggerManager.Logger.Error("Adding error {0}", ex.Message);
                throw;
            }
        }

        public async Task UpdateAsync(NodeData entity)
        {
            try
            {
                _context.Set<NodeData>().Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
                _context.SaveChanges();
                await Task.CompletedTask.WaitAsync(CancellationToken.None);
            }
            catch (Exception ex)
            {
                LoggerManager.Logger.Error("Updating error {0}", ex.Message);
                throw;
            }
        }


        public async Task<NodeData> FindByIdAsync(string id)
        {
            var t = await _context.SampleNodes.Where(c => c.NodeId == id).OrderBy(c => c.StoreTime).ToListAsync();
            if (_context.SampleNodes.Count() > 500)
            {
                Console.WriteLine("DataAccess: ");
            }
            return t.LastOrDefault();
        }

        public async Task DeleteAsync(NodeData entity)
        {
            _context.SampleNodes.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<NodeData>> ListAllAsync()
        {
            return await _context.SampleNodes.ToListAsync();
        }
    }
}
