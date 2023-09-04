using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QIA.Plugin.OpcClient.Core.Settings;
using QIA.Plugin.OpcClient.Entities;

namespace QIA.Plugin.OpcClient.Database
{
    public class SampleNodeEntity : IEntityTypeConfiguration<NodeData>
    {
        public void Configure(EntityTypeBuilder<NodeData> builder)
        {
            builder.ToTable(AppSettings.GetTargetTbl);
            builder.HasKey(c => c.Id);
            builder.HasIndex(e => e.NodeId);
            builder.Property(e => e.NodeType)
                .HasConversion<string>();
        }
    }
}
