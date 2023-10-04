using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Qia.Opc.Domain.Common;
using Qia.Opc.Domain.Entities;

namespace Qia.Opc.Persistence
{
    public class NodeReferenceEntityEntity : IEntityTypeConfiguration<NodeReferenceEntity>
	{
		public void Configure(EntityTypeBuilder<NodeReferenceEntity> builder)
		{
			builder.HasKey(e => e.NodeId);
			builder.HasIndex(e => e.NodeId);
		}
	}

	public class NodeDataEntity : IEntityTypeConfiguration<NodeData>
	{
		public void Configure(EntityTypeBuilder<NodeData> builder)
		{
			builder.ToTable(StaticSettings.GetTargetTbl);
			builder.HasKey(e => e.Id);
			builder.HasIndex(e => e.NodeId).IsUnique();
		}
	}
}
