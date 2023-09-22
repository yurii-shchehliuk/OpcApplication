using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QIA.Plugin.OpcClient.Entities;

namespace QIA.Plugin.OpcClient.Database
{
	public class BaseNodeEntity : IEntityTypeConfiguration<BaseNode>
	{
		public void Configure(EntityTypeBuilder<BaseNode> builder)
		{
			builder.ToTable("NodesConfigs");
			builder.HasIndex(e => e.NodeId);
		}
	}

	public class NodeDataEntity : IEntityTypeConfiguration<NodeData>
	{
		public void Configure(EntityTypeBuilder<NodeData> builder)
		{
			builder.ToTable(AppSettings.GetTargetTbl);
			builder.HasIndex(e => e.NodeId);
		}
	}

	public class AzureEventHubEntity : IEntityTypeConfiguration<AzureEventHub>
	{
		public void Configure(EntityTypeBuilder<AzureEventHub> builder)
		{
			builder.HasOne(a => a.AppSettings)
					.WithOne(b => b.AzureEventHub)
					.HasForeignKey<AppSettings>(b => b.AzureEventHubId);
		}
	}
}
