using Microsoft.Extensions.Configuration;
using QIA.Plugin.OpcClient.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QIA.Plugin.OpcClient.Core.Settings
{
    public class AppSettings : IAppSettings
    {
        public static string GetTargetTbl = "";
        public AppSettings(IConfiguration configuration)
        {
            configuration.Bind("Workers:OPC_UA.QGH01:Config", this);
            AppSettings.GetTargetTbl = TargetTable;
        }

        public string DbConnectionString { get; set; }
        public string OpcUrl { get; set; }
        public bool CreateFullTree { get; set; }
        public bool SaveToAzure { get; set; }
        public bool SaveToDb { get; set; }
        public string TargetTable { get; set; }
        public AzureEventHub AzureEventHub { get; set; }
        public HashSet<NodeConfig> NodeManager { get; set; } = new HashSet<NodeConfig>();
    }
}
