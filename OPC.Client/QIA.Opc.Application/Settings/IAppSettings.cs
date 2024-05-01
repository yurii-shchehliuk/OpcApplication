namespace QIA.Opc.Application.Settings;

public interface IAppSettings
{
    string DbConnectionString { get; }
    bool SaveToAzure { get; }
    string TargetTable { get; }
    AzureEventHub AzureEventHub { get; }
    string AzureSignalR { get; }
    string[] CORS { get; }
}
