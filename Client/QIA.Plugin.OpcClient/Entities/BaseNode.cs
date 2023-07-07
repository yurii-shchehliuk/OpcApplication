using System;
using System.ComponentModel.DataAnnotations;

namespace QIA.Plugin.OpcClient.Entities
{
    /// <summary>
    /// DB node model
    /// </summary>
    public abstract class BaseNode
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string NodeId { get; set; }
        public DateTime StoreTime { get; set; } = DateTime.UtcNow;
        public string Value { get; set; }
        public string Name { get; set; }
        public NodeType NodeType { get; set; }
        public uint MSecs { get; set; } = 0;
        public uint Range { get; set; } = 1;
    }

    public class SampleNode : BaseNode
    {
    }

    //TODO: use LiteSQL to store temp
    public class TempNode : BaseNode
    {
    }


}
