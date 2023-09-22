using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.IO;
using System.Text.Json;
using QIA.Plugin.OpcClient.Entities;
using QIA.Plugin.OpcClient.Core;

namespace QIA.Plugin.OpcClient.DTO
{
    public class StandaloneConfigurationDTO : IAppSettings
    {
        public StandaloneConfigurationDTO(IConfiguration config)
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
        public SignalRSettings SignalR { get; set; }
        public AzureEventHub AzureEventHub { get; set; }
    }
}
