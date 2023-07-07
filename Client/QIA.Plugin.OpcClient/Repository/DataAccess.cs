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

        public async Task AddAsync(SampleNode entity)
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
                Console.WriteLine("Adding error {0}", ex.Message);
                throw;
            }
        }

        public async Task UpdateAsync(SampleNode entity)
        {
            try
            {
                _context.Set<SampleNode>().Attach(entity);
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
        public AppConfig GetAppConfig()
        {
            try
            {
                //var conf = _context.AppConfig.AsEnumerable().LastOrDefault();
                //if (conf != null)
                //{
                //    return conf;
                //}

                var typesData = File.ReadAllText(Directory.GetCurrentDirectory() + @"/qia.opc/AppConfig.json");
                var types = JsonSerializer.Deserialize<List<AppConfig>>(typesData);


                return types.LastOrDefault();
            }
            catch (Exception ex)
            {
                //sql exception
                LoggerManager.Logger.Warning("GetAppConfig: {0}", ex.Message);

                //var typesData = File.ReadAllText(Directory.GetCurrentDirectory() + @"/Database/SeedData/AppConfig.json");
                //var types = JsonSerializer.Deserialize<List<AppConfig>>(typesData);

                return new AppConfig();
            }

        }

        public async Task<SampleNode> FindByIdAsync(string id)
        {
            var t = await _context.SampleNodes.Where(c => c.NodeId == id).OrderBy(c => c.StoreTime).ToListAsync();
            return t.LastOrDefault();
        }

        public async Task DeleteAsync(SampleNode entity)
        {
            _context.SampleNodes.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<SampleNode>> ListAllAsync()
        {
            return await _context.SampleNodes.ToListAsync();
        }
    }
}
