using Opc.Ua;
using Opc.Ua.Configuration;
using Qia.Opc.Domain.Core;
using System.Diagnostics;
using System.Reflection;

namespace Qia.Opc.OPCUA.Connector
{
	using static LoggerManager;
	public partial class ApplicationConfigBuilder
	{
		public ApplicationConfiguration ApplicationConfiguration { get; private set; }
		private ApplicationInstance application { get; set; }

		private static readonly Lazy<ApplicationConfigBuilder> _instance = new Lazy<ApplicationConfigBuilder>(() => new ApplicationConfigBuilder());
		public static ApplicationConfigBuilder Instance => _instance.Value;
		public ApplicationConfigBuilder()
		{

		}

		public async Task Init()
		{
			LoggerManager.Logger.Information("Configuring UA Client");
			LoggerManager.Logger.Information($"bpc V{FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion} starting up...");

			// configure application with security
			ApplicationConfiguration = new ApplicationConfiguration()
			{
				ApplicationName = "QiagenOpcClient",
				ApplicationUri = Utils.Format(@"urn:{0}:QiagenOpcClient", System.Net.Dns.GetHostName()),
				ApplicationType = ApplicationType.Client,

				TransportConfigurations = new TransportConfigurationCollection(),
				TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },

				// add default client configuration
				ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 },
				ServerConfiguration = new ServerConfiguration
				{
					SecurityPolicies = new ServerSecurityPolicyCollection
					{
						new ServerSecurityPolicy
						{
							// Disable security
							SecurityMode = MessageSecurityMode.None,
							SecurityPolicyUri = SecurityPolicies.None
						},
					},
				},

			};

			// configure OPC stack tracing
			ApplicationConfiguration.TraceConfiguration = new TraceConfiguration
			{
				TraceMasks = OpcStackTraceMask
			};
			ApplicationConfiguration.TraceConfiguration.ApplySettings();
			Utils.Tracing.TraceEventHandler += new EventHandler<TraceEventArgs>(LoggerOpcUaTraceHandler);

			await InitApplicationSecurityAsync();

			await ShowCertificateStoreInformationAsync();


			await ApplicationConfiguration.Validate(ApplicationType.Client);

			//create app instance
			application = new ApplicationInstance
			{
				ApplicationName = "QiagenOpcClient",
				ApplicationType = ApplicationType.Client,
				ApplicationConfiguration = ApplicationConfiguration
			};

			var valid = application.CheckApplicationInstanceCertificate(false, 0).GetAwaiter().GetResult();
			LoggerManager.Logger.Information("Own certificate is: " + (valid ? "valid" : "not valid"));

			//AppConfiguration.OnDiscoverEndpoints(application);
		}
	}
}
