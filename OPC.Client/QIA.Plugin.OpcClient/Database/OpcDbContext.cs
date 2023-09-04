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

        public DbSet<NodeData> SampleNodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
