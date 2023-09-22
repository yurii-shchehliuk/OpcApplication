using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using QIA.Plugin.OpcClient.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QIA.Plugin.OpcClient.Entities
{
    public class AppSettings : BaseEntity, IAppSettings
    {
        public static string GetTargetTbl = "";

        public string Name { get; set; }
        public string DbConnectionString { get; set; }
        public string OpcUrl { get; set; }
        public bool CreateFullTree { get; set; }
        public bool SaveToAzure { get; set; }
        public bool SaveToDb { get; set; }
        public string TargetTable { get; set; }
        public int? SignalRId { get; set; }
        public virtual SignalRSettings SignalR { get; set; } = new SignalRSettings();
        public int? AzureEventHubId { get; set; }
        public virtual AzureEventHub AzureEventHub { get; set; } = new AzureEventHub();
        [JsonIgnore]
        public virtual Group Group { get; set; }


    }

    public class SignalRSettings : BaseEntity
    {
        public string HubUrl { get; set; } = "https://localhost:7027/chathub";
        [JsonIgnore] 
        public virtual AppSettings AppSettings { get; set; }
    }
}
