using Microsoft.EntityFrameworkCore;
using Qia.Opc.Domain.Entities;
using System.Reflection;

namespace Qia.Opc.Persistence
{
	public class OpcDbContext : DbContext
	{
		public OpcDbContext(DbContextOptions<OpcDbContext> options) : base(options)
		{
		}

		public DbSet<NodeReferenceEntity> NodesReferences { get; set; }
		public DbSet<NodeData> NodeConfigMonitorValues { get; set; }
		public DbSet<SessionEntity> Sessions { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
		}
	}
}
