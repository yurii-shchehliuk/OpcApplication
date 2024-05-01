namespace QIA.Opc.Infrastructure.DataAccess;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Qia.Opc.Domain.Entities;
using QIA.Opc.Application.Settings;
using QIA.Opc.Domain.Entities;

public class SessionEntityConfig : IEntityTypeConfiguration<SessionConfig>
{
    public void Configure(EntityTypeBuilder<SessionConfig> builder)
    {
        builder.HasKey(s => s.Guid);

        builder.HasMany(c => c.SubscriptionConfigs)
               .WithOne()
               .HasForeignKey(sc => sc.SessionGuid)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

public class SubscriptionConfigEntity : IEntityTypeConfiguration<SubscriptionConfig>
{
    public void Configure(EntityTypeBuilder<SubscriptionConfig> builder)
    {
        builder.HasKey(s => s.Guid);

        builder.HasMany(s => s.MonitoredItemsConfig)
               .WithOne()
               .HasForeignKey(mic => mic.SubscriptionGuid)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.MonitoredItemsValue)
                .WithOne()
                .HasForeignKey(miv => miv.SubscriptionGuid)
                .OnDelete(DeleteBehavior.Cascade);
    }
}

public class MonitoredItemConfigEntity : IEntityTypeConfiguration<MonitoredItemConfig>
{
    public void Configure(EntityTypeBuilder<MonitoredItemConfig> builder)
    {
        builder.HasKey(e => e.Guid);
        builder.Property(e => e.Guid).ValueGeneratedOnAdd();

        builder.HasIndex(e => e.StartNodeId);
    }
}

public class MonitoredItemValueEntity : IEntityTypeConfiguration<MonitoredItemValue>
{
    public void Configure(EntityTypeBuilder<MonitoredItemValue> builder)
    {
        builder.ToTable(StaticSettings.NodeValuesTblName);
        builder.HasKey(e => e.Guid);
        builder.Property(e => e.Guid).ValueGeneratedOnAdd();
        builder.HasIndex(e => e.StartNodeId);
    }
}
