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
			builder.HasKey(s => s.SessionGuidId);
			builder.HasIndex(e => e.SessionGuidId).IsUnique();
			builder.Ignore(c => c.State);
		}
	}

	public class SubscriptionConfigEntity : IEntityTypeConfiguration<SubscriptionConfig>
	{
		public void Configure(EntityTypeBuilder<SubscriptionConfig> builder)
		{
			builder.HasKey(s => s.SubscriptionGuidId);
			builder.HasIndex(e => e.SubscriptionGuidId).IsUnique();

			builder.HasOne(c => c.Session)
					.WithMany(c => c.SubscriptionConfigs)
			   .HasForeignKey(c => c.SessionGuidId)
					.OnDelete(DeleteBehavior.Cascade);
		}
	}


	public class MonitoredItemConfigEntity : IEntityTypeConfiguration<MonitoredItemConfig>
	{
		public void Configure(EntityTypeBuilder<MonitoredItemConfig> builder)
		{
			builder.HasKey(e => e.GuidId);
			builder.HasIndex(e => e.StartNodeId);
			builder.HasOne(c => c.Subscription)
					.WithMany(c => c.MonitoredItemsConfig)
					.HasForeignKey(c => c.SubscriptionGuidId)
					.OnDelete(DeleteBehavior.Cascade);
		}
	}

	public class MonitoredItemValueEntity : IEntityTypeConfiguration<MonitoredItemValue>
	{
		public void Configure(EntityTypeBuilder<MonitoredItemValue> builder)
		{
			//builder.ToTable(StaticSettings.GetTargetTbl);
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
