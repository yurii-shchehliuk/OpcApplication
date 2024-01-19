using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Qia.Opc.Domain.Common;
using Qia.Opc.Domain.Entities;
using QIA.Opc.Domain.Entities;

namespace Qia.Opc.Persistence
{
	public class SessionEntityConfig : IEntityTypeConfiguration<SessionEntity>
	{
		public void Configure(EntityTypeBuilder<SessionEntity> builder)
		{
			builder.HasKey(s => s.Guid);
			builder.Ignore(c => c.State);

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
			builder.HasKey(e => e.ServerId);
			builder.HasIndex(e => e.StartNodeId);
		}
	}

	public class TreeContainerEntity : IEntityTypeConfiguration<TreeContainer>
	{
		public void Configure(EntityTypeBuilder<TreeContainer> builder)
		{
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Data).HasColumnType<string>("VARCHAR(MAX)");
		}
	}
}
