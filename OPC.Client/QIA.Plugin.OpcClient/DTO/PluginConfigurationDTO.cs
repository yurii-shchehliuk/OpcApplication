using Microsoft.Extensions.Configuration;
using QIA.Library.Worker;
using QIA.Plugin.OpcClient.Core;
using QIA.Plugin.OpcClient.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QIA.Plugin.OpcClient.DTO
{
    public class PluginConfigurationDTO : QIAWorkerConfig, IAppSettings
    {
        public PluginConfigurationDTO(IConfiguration configuration, string section) : base(configuration, section)
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
        public SignalRSettings SignalR { get; set; }
        public AzureEventHub AzureEventHub { get; set; }
    }
}
