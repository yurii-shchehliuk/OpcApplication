namespace QIA.Opc.Application.Settings;
using Microsoft.Extensions.Configuration;

public class AppSettings : IAppSettings
{
    public AppSettings(IConfiguration configuration)
    {
        configuration.Bind("Config", this);
        StaticSettings.NodeValuesTblName = TargetTable;
    }

    public string DbConnectionString { get; set; } = string.Empty;
    public bool SaveToAzure { get; set; }
    public string TargetTable { get; set; } = string.Empty;
    public string AzureSignalR { get; set; } = string.Empty;
    public string[] CORS { get; set; }
    public virtual AzureEventHub AzureEventHub { get; set; }
}

public class StaticSettings
{
    public static string NodeValuesTblName = "NodeValues";
}
