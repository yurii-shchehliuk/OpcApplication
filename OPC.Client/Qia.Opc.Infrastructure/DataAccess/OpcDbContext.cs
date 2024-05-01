namespace QIA.Opc.Infrastructure.DataAccess;

using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Qia.Opc.Domain.Entities;
using QIA.Opc.Domain.Entities;

public class OpcDbContext : DbContext
{
    public OpcDbContext(DbContextOptions<OpcDbContext> options) : base(options)
    {
    }

    public DbSet<SessionConfig> Sessions { get; set; }
    public DbSet<SubscriptionConfig> SubscriptionConfigs { get; set; }
    public DbSet<MonitoredItemConfig> MonitoredItemConfigs { get; set; }
    public DbSet<MonitoredItemValue> MonitoredItemValues { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
