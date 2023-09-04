using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using QIA.Plugin.OpcClient.Core;
using System;
using System.Diagnostics;
using System.Reflection;

namespace QIA.Plugin.OpcClient
{
    public class OpcConfiguration : IDisposable
    {
        private static OpcConfiguration _instance;
        public static Session OpcUaClientSession { get; set; } //TODO: replace with OpcSession
        private static ApplicationConfiguration config;
        public ApplicationInstance application;

        private OpcConfiguration()
        {
            Init();
        }

        public static OpcConfiguration GetInstance()
        {
            _instance ??= new OpcConfiguration();
            return _instance;
        }

        private void Init()
        {
            LoggerManager.Logger.Information("Configuring UA Client");
            LoggerManager.Logger.Information($"bpc V{FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion} starting up...");

            // configure application with security
            config = new ApplicationConfiguration()
            {
                ApplicationName = "QiagenOpcClient",
                ApplicationUri = Utils.Format(@"urn:{0}:QiagenOpcClient", System.Net.Dns.GetHostName()),
                ApplicationType = ApplicationType.Client,
                SecurityConfiguration = new SecurityConfiguration
                {
                    ApplicationCertificate = new CertificateIdentifier { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\MachineDefault", SubjectName = "QiagenOpcClient" },
                    TrustedIssuerCertificates = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Certificate Authorities" },
                    TrustedPeerCertificates = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Applications" },
                    RejectedCertificateStore = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\RejectedCertificates" },
                    AutoAcceptUntrustedCertificates = true,

                },
                TransportConfigurations = new TransportConfigurationCollection(),
                TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },

                ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 },
                TraceConfiguration = new TraceConfiguration(),
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

            config.Validate(ApplicationType.Client).GetAwaiter().GetResult();
            if (config.SecurityConfiguration.AutoAcceptUntrustedCertificates)
            {
                config.CertificateValidator.CertificateValidation += (s, e) => { e.Accept = e.Error.StatusCode == StatusCodes.BadCertificateUntrusted; };
            }

            //create app instance
            application = new ApplicationInstance
            {
                ApplicationName = "QiagenOpcClient",
                ApplicationType = ApplicationType.Client,
                ApplicationConfiguration = config
            };

            application.CheckApplicationInstanceCertificate(false, 2048).GetAwaiter().GetResult();

            //AppConfiguration.OnDiscoverEndpoints(application);

            LoggerManager.Logger.Information("Connecting to: " + Extensions.ReadServerUrl());
        }


        /// <summary>
        /// Session factory
        /// </summary>
        /// <remarks>TODO: TBD</remarks>
        public void InitSession(string url = "")
        {
            //TODO: extend with multiple sessions
            if (string.IsNullOrEmpty(url))
                url = Extensions.ReadServerUrl();

            try
            {
                var selectedEndpoint_ = CoreClientUtils.SelectEndpoint(application.ApplicationConfiguration, url, useSecurity: true);

                // User identity anonymous
                OpcUaClientSession = Session.Create(config, new ConfiguredEndpoint(null, selectedEndpoint_, EndpointConfiguration.Create(config)),
                    false, "", 60000, new UserIdentity(new AnonymousIdentityToken()), null).GetAwaiter().GetResult();

                LoggerManager.Logger.Information($"Session started");

            }
            catch (ServiceResultException ex)
            {
                LoggerManager.Logger.Error(ex, "Cannot connect to the server:" + ex.Message);
                throw ex;
            }
        }

        public void Dispose()
        {
            OpcUaClientSession.Dispose();
            OpcUaClientSession = null;
            GC.SuppressFinalize(this);
        }

        private enum StatusType
        {
            Ok = 0,
            Warning = 1,
            Error = 2
        }
    }
}
