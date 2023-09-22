using QIA.Plugin.OpcClient.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QIA.Plugin.OpcClient.Core
{
    public interface IAppSettings
    {
        string DbConnectionString { get; set; }
        string OpcUrl { get; set; }
        bool CreateFullTree { get; set; }
        bool SaveToAzure { get; set; }
        bool SaveToDb { get; set; }
        string TargetTable { get; set; }
        SignalRSettings SignalR { get; set; }
        AzureEventHub AzureEventHub { get; set; }

        //void PopulateSettings();
    }
}
