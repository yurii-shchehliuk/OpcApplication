namespace Qia.Opc.Domain.Common
{
    public interface IAppSettings
    {
        string DbConnectionString { get; }
        bool SaveToAzure { get; }
        string TargetTable { get; }
		string KeyVaultUri { get; }
        AzureEventHub AzureEventHub { get; }
	}
}
