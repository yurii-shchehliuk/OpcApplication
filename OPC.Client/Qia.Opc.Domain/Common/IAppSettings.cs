using QIA.Opc.Domain.Entities;

namespace Qia.Opc.Domain.Common
{
	public interface IAppSettings
	{
		string DbConnectionString { get; }
		bool SaveToAzure { get; }
		string TargetTable { get; }
		AzureEventHub AzureEventHub { get; }
	}
}
