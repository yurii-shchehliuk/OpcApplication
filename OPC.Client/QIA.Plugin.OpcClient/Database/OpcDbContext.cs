using Microsoft.EntityFrameworkCore;
using QIA.Plugin.OpcClient.Entities;
using System.Reflection;

namespace QIA.Plugin.OpcClient.Database
{
    public class OpcDbContext : DbContext
    {
        public OpcDbContext(DbContextOptions<OpcDbContext> options) : base(options)
        {
        }

        public DbSet<BaseNode> NodesConfig { get; set; }
        public DbSet<NodeData> MonitorNodes{ get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<AppSettings> AppSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
