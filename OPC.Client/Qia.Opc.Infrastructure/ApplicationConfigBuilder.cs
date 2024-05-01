namespace Qia.Opc.OPCUA.Connector;

using System.Diagnostics;
using System.Net;
using System.Reflection;
using global::Opc.Ua;
using global::Opc.Ua.Configuration;
using static QIA.Opc.Infrastructure.Application.LoggerManager;

public partial class ApplicationConfigBuilder
{
    public ApplicationConfiguration ApplicationConfiguration { get; private set; }
    private ApplicationInstance Application { get; set; }

    public async Task Init()
    {
        Logger.Information("Configuring UA Client");
        Logger.Information($"bpc V{FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion} starting up...");

        // configure application with security
        ApplicationConfiguration = new ApplicationConfiguration
        {
            ApplicationName = "QiagenOpcClient",
            ApplicationUri = Utils.Format(@"urn:{0}:QiagenOpcClient", Dns.GetHostName()),
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
            // configure OPC stack tracing
            TraceConfiguration = new TraceConfiguration
            {
                TraceMasks = OpcStackTraceMask
            }
        };

        ApplicationConfiguration.TraceConfiguration.ApplySettings();
        Utils.Tracing.TraceEventHandler += new EventHandler<TraceEventArgs>(LoggerOpcUaTraceHandler);

        InitSecurity();

        //LogSecurity();

        await LoadAppArguments();

        await InitCertificateAsync();

        await ShowCertificateStoreInformationAsync();

        await ApplicationConfiguration.Validate(ApplicationType.Client);

        //create app instance
        Application = new ApplicationInstance
        {
            ApplicationName = "QiagenOpcClient",
            ApplicationType = ApplicationType.Client,
            ApplicationConfiguration = ApplicationConfiguration
        };

        var valid = Application.CheckApplicationInstanceCertificate(false, 0).GetAwaiter().GetResult();
        Logger.Information("Own certificate is: " + (valid ? "valid" : "not valid"));

        //AppConfiguration.OnDiscoverEndpoints(application);
    }
}
