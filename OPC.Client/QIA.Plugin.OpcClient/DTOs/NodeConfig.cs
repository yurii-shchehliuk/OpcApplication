using QIA.Plugin.OpcClient.Entities;

namespace QIA.Plugin.OpcClient.DTOs
{
    /// <summary>
    /// JSON model
    /// </summary>
    public class NodeConfig
    {
        public string NodeId { get; set; }
        public string Name { get; set; }
        public uint Range { get; set; } = 1;
        public uint Msecs { get; set; } = 1000;
        public NodeType NodeType { get; set; }

        //private bool Update_Or_Store_OnSubscribe?;
    }
}
