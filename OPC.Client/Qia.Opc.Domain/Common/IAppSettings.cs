namespace Qia.Opc.Domain.Common
{
    public interface IAppSettings
    {
        string DbConnectionString { get; set; }
        bool SaveToAzure { get; set; }
        string TargetTable { get; set; }
        AzureEventHub AzureEventHub { get; set; }
    }
}
