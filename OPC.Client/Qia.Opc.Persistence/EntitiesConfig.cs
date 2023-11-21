using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Qia.Opc.Domain.Common;
using Qia.Opc.Domain.Entities;

namespace Qia.Opc.Persistence
{
	public class SessionEntityConfig : IEntityTypeConfiguration<SessionEntity>
	{
		public void Configure(EntityTypeBuilder<SessionEntity> builder)
		{
			builder.HasKey(c => c.Id);
			builder.Ignore(c => c.State);
			builder.Ignore(c => c.SessionNodeId);
			builder.Ignore(c => c.SessionId);
		}
	}
	
	public class NodeReferenceEntityEntity : IEntityTypeConfiguration<NodeReferenceEntity>
	{
		public void Configure(EntityTypeBuilder<NodeReferenceEntity> builder)
		{
			builder.HasKey(e => e.NodeId);
			builder.HasOne(c => c.SessionEntity)
					.WithMany(c=>c.NodeConfigs)
					.HasForeignKey(c=>c.SessionEntityId)
					.OnDelete(DeleteBehavior.Cascade);
		}
	}

	public class NodeDataEntity : IEntityTypeConfiguration<NodeValue>
	{
		public void Configure(EntityTypeBuilder<NodeValue> builder)
		{
			builder.ToTable(StaticSettings.GetTargetTbl);
			builder.HasKey(e => e.Id);
			builder.HasIndex(e => e.NodeId);
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
