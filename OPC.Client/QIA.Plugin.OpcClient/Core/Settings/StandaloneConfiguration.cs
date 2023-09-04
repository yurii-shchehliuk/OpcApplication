using Microsoft.Extensions.Configuration;
using QIA.Plugin.OpcClient.DTOs;
using System.Collections.Generic;
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.IO;
using System.Text.Json;

namespace QIA.Plugin.OpcClient.Core.Settings
{
    public class StandaloneConfiguration : IAppSettings
    {
        public StandaloneConfiguration(IConfiguration config)
        {
            config.Bind("Workers:OPC_UA.QGH01:Config", this);
            AppSettings.GetTargetTbl = TargetTable;
        }

        public string DbConnectionString { get; set; }
        public string OpcUrl { get; set; }
        public bool CreateFullTree { get; set; } = true;
        public bool SaveToAzure { get; set; } = false;
        public bool SaveToDb { get; set; }
        public string TargetTable { get; set; }
        public AzureEventHub AzureEventHub { get; set; }
        public HashSet<NodeConfig> NodeManager { get; set; } = new HashSet<NodeConfig>();
    }
}
