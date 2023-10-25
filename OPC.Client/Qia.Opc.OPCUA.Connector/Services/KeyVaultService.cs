using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using Qia.Opc.Domain.Common;
using Qia.Opc.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Qia.Opc.OPCUA.Connector.Services
{
	public class KeyVaultService
	{
		private readonly CertificateClient _client;

		public KeyVaultService(IAppSettings appSettings, IHostEnvironment environment)
		{
			if (environment.IsDevelopment())
			{
				string AZURE_TENANT_ID = Environment.GetEnvironmentVariable("AZURE_TENANT_ID", EnvironmentVariableTarget.User);
				string AZURE_CLIENT_ID = Environment.GetEnvironmentVariable("AZURE_CLIENT_ID", EnvironmentVariableTarget.User);
				string AZURE_CLIENT_SECRET = Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET", EnvironmentVariableTarget.User);
				var clientCredential = new ClientSecretCredential(AZURE_TENANT_ID, AZURE_CLIENT_ID, AZURE_CLIENT_SECRET);
				_client = new CertificateClient(new Uri(appSettings.KeyVaultUri), clientCredential);

			}
			else
			{
				var credential = new DefaultAzureCredential(); //make sure your system variables are setted for EnvironmentCredential case
				_client = new CertificateClient(new Uri(appSettings.KeyVaultUri), credential);
			}
		}

		public async Task<X509Certificate2> GetCertificateAsync(string certificateName)
		{
			try
			{
				var secret = await _client.GetCertificateAsync(certificateName);
				X509Certificate2 certificate = new X509Certificate2(secret.Value.Cer);
				return certificate;
			}
			catch (RequestFailedException ex)
			{
				LoggerManager.Logger.Error(ex.Message, ex);
			}
			catch (Exception ex)
			{
				LoggerManager.Logger.Error(ex.Message, ex);
			}

			return null;
		}

		public async Task StoreCertificateAsync(string certificateName, X509Certificate2 certificate, string password = null)
		{
			try
			{
				//byte[] pfxBytes = certificate.Export(X509ContentType.Cert);

				byte[] pfxBytes = certificate.Export(X509ContentType.Pfx, password);

				ImportCertificateOptions options = new ImportCertificateOptions(certificateName, pfxBytes)
				{
					Password = password,
					
				};
				await _client.ImportCertificateAsync(options);
			}
			catch (RequestFailedException e)
			{
				LoggerManager.Logger.Error(e.Message, e);
			}
			catch (Exception ex)
			{
				LoggerManager.Logger.Error(ex.Message, ex);
			}

		}
	}
}
