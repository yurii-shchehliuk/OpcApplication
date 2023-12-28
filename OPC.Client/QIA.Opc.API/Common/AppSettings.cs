using Microsoft.Extensions.Configuration;

namespace Qia.Opc.Domain.Common
{
	public class AppSettings : IAppSettings
	{
		public AppSettings(IConfiguration configuration)
		{
			configuration.Bind("Config", this);
			StaticSettings.NodeValuesTblName = TargetTable;
		}

		public string DbConnectionString { get; set; }
		public bool SaveToAzure { get; set; }
		public string TargetTable { get; set; }
		public virtual AzureEventHub AzureEventHub { get; set; }
	}

	public class StaticSettings
	{
		public static string NodeValuesTblName = "NodeData";
	}
}
