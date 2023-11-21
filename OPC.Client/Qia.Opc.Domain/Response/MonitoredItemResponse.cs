using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QIA.Opc.Domain.Response
{
    /// <summary>
    /// Real-time node value
    /// </summary>
    public class MonitoredItemResponse
    {
        public string DisplayName { get; set; }
        public int SamplingInterval { get; set; }
        public uint QueueSize { get; set; }
        public string StartNodeId { get; set; }
        public object Value { get; set; }
        public DateTime? SourceTime { get; set; }
    }
}
