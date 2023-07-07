using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QIA.Plugin.OpcClient.Entities;

namespace QIA.Plugin.OpcClient.Database
{
    public class SampleNodeEntity : IEntityTypeConfiguration<SampleNode>
    {
        public void Configure(EntityTypeBuilder<SampleNode> builder)
        {
            builder.ToTable("LSLU1_OPC_UA");
            builder.HasKey(c => c.Id);
            builder.HasIndex(e => e.NodeId);
            builder.Property(e => e.NodeType)
                .HasConversion<string>();
        }
    }
}
