﻿
using Opc.Ua;
using System.Security.Cryptography.X509Certificates;

namespace Qia.Opc.OPCUA.Connector
{
	using global::Opc.Ua.Security.Certificates;
	using Qia.Opc.Domain.Core;
	using Qia.Opc.OPCUA.Connector.Services;
	using System.Collections.Generic;
	using System.IO;
	using System.Threading.Tasks;

	/// <summary>
	/// Class for OPC Application configuration. Here the security relevant configuration.
	/// </summary>
	public partial class ApplicationConfigBuilder
	{
		/// <summary>
		/// Add own certificate to trusted peer store.
		/// </summary>
		public static bool TrustMyself { get; set; } = false;

		/// <summary>
		/// Certficate store configuration for own, trusted peer, issuer and rejected stores.
		/// </summary>
		public static string OpcOwnCertStoreType { get; set; } = "AzureKeyVault";//CertificateStoreType.X509Store;
		public static string OpcOwnCertDirectoryStorePathDefault => "pki/own";
		public static string OpcOwnCertX509StorePathDefault => "CurrentUser\\UA_MachineDefault";

		public static string OpcTrustedCertDirectoryStorePathDefault => "pki/trusted";
		public static string OpcTrustedCertStorePath { get; set; } = OpcTrustedCertDirectoryStorePathDefault;

		public static string OpcRejectedCertDirectoryStorePathDefault => "pki/rejected";
		public static string OpcRejectedCertStorePath { get; set; } = OpcRejectedCertDirectoryStorePathDefault;

		public static string OpcIssuerCertDirectoryStorePathDefault => "pki/issuer";
		public static string OpcIssuerCertStorePath { get; set; } = OpcIssuerCertDirectoryStorePathDefault;

		/// <summary>
		/// Accept certs of the clients automatically.
		/// </summary>
		public static bool AutoAcceptCerts { get; set; } = true;
		/// <summary>
		/// Specify if session should use a secure endpoint.
		/// </summary>
		public static bool UseSecurity { get; set; } = false;
		/// <summary>
		/// Show CSR information during startup.
		/// </summary>
		public static bool ShowCreateSigningRequestInfo { get; set; } = false;

		/// <summary>
		/// Update application certificate.
		/// </summary>
		public static string NewCertificateBase64String { get; set; } = null;
		public static string NewCertificateFileName { get; set; } = null;
		public static string CertificatePassword { get; set; } = string.Empty;

		/// <summary>
		/// If there is no application cert installed we need to install the private key as well.
		/// </summary>
		public static string PrivateKeyBase64String { get; set; } = null;
		public static string PrivateKeyFileName { get; set; } = null;

		/// <summary>
		/// Issuer certificates to add.
		/// </summary>
		public static List<string> IssuerCertificateBase64Strings = null;
		public static List<string> IssuerCertificateFileNames = null;

		/// <summary>
		/// Trusted certificates to add.
		/// </summary>
		public static List<string> TrustedCertificateBase64Strings = null;
		public static List<string> TrustedCertificateFileNames = null;

		/// <summary>
		/// CRL to update/install.
		/// </summary>
		public static string CrlFileName { get; set; } = null;
		public static string CrlBase64String { get; set; } = null;

		/// <summary>
		/// Thumbprint of certificates to delete.
		/// </summary>
		public static List<string> ThumbprintsToRemove = null;
		/// <summary>
		/// Configures OPC stack certificates.
		/// </summary>
		public async Task InitApplicationSecurityAsync(KeyVaultService keyVaultService)
		{
			// security configuration
			ApplicationConfiguration.SecurityConfiguration = new SecurityConfiguration
			{
				// configure trusted issuer certificates store
				TrustedIssuerCertificates = new CertificateTrustList()
			};
			//ApplicationConfiguration.SecurityConfiguration.TrustedIssuerCertificates.StoreType = OpcOwnCertStoreType;
			//ApplicationConfiguration.SecurityConfiguration.TrustedIssuerCertificates.StorePath = OpcIssuerCertStorePath;
			LoggerManager.Logger.Information($"Trusted Issuer store type is: {ApplicationConfiguration.SecurityConfiguration.TrustedIssuerCertificates.StoreType}");
			LoggerManager.Logger.Information($"Trusted Issuer Certificate store path is: {ApplicationConfiguration.SecurityConfiguration.TrustedIssuerCertificates.StorePath}");

			// configure trusted peer certificates store
			ApplicationConfiguration.SecurityConfiguration.TrustedPeerCertificates = new CertificateTrustList();
			//{
			//	StoreType = OpcOwnCertStoreType,
			//	StorePath = OpcTrustedCertStorePath
			//};
			LoggerManager.Logger.Information($"Trusted Peer Certificate store type is: {ApplicationConfiguration.SecurityConfiguration.TrustedPeerCertificates.StoreType}");
			LoggerManager.Logger.Information($"Trusted Peer Certificate store path is: {ApplicationConfiguration.SecurityConfiguration.TrustedPeerCertificates.StorePath}");

			// configure rejected certificates store
			ApplicationConfiguration.SecurityConfiguration.RejectedCertificateStore = new CertificateTrustList();
			//{
			//	StoreType = OpcOwnCertStoreType,
			//	StorePath = OpcRejectedCertStorePath
			//};

			LoggerManager.Logger.Information($"Rejected certificate store type is: {ApplicationConfiguration.SecurityConfiguration.RejectedCertificateStore.StoreType}");
			LoggerManager.Logger.Information($"Rejected Certificate store path is: {ApplicationConfiguration.SecurityConfiguration.RejectedCertificateStore.StorePath}");

			// we allow SHA1 certificates for now as many OPC Servers still use them
			ApplicationConfiguration.SecurityConfiguration.RejectSHA1SignedCertificates = false;
			LoggerManager.Logger.Information($"Rejection of SHA1 signed certificates is {(ApplicationConfiguration.SecurityConfiguration.RejectSHA1SignedCertificates ? "enabled" : "disabled")}");

			// we allow a minimum key size of 1024 bit, as many OPC UA servers still use them
			ApplicationConfiguration.SecurityConfiguration.MinimumCertificateKeySize = 1024;
			LoggerManager.Logger.Information($"Minimum certificate key size set to {ApplicationConfiguration.SecurityConfiguration.MinimumCertificateKeySize}");

			// configure application certificate store
			ApplicationConfiguration.SecurityConfiguration.ApplicationCertificate = new CertificateIdentifier()
			{
				//StoreType = OpcOwnCertStoreType,
				//StorePath = appSettings.KeyVaultUri,
				SubjectName = ApplicationConfiguration.ApplicationName
			};
			//LoggerManager.Logger.Information($"Application Certificate store type is: {ApplicationConfiguration.SecurityConfiguration.ApplicationCertificate.StoreType}");
			//LoggerManager.Logger.Information($"Application Certificate store path is: {ApplicationConfiguration.SecurityConfiguration.ApplicationCertificate.StorePath}");
			LoggerManager.Logger.Information($"Application Certificate subject name is: {ApplicationConfiguration.SecurityConfiguration.ApplicationCertificate.SubjectName}");

			// handle cert validation
			if (AutoAcceptCerts)
			{
				LoggerManager.Logger.Warning("WARNING: Automatically accepting certificates. This is a security risk.");
				ApplicationConfiguration.SecurityConfiguration.AutoAcceptUntrustedCertificates = true;
			}
			ApplicationConfiguration.CertificateValidator = new CertificateValidator();
			ApplicationConfiguration.CertificateValidator.CertificateValidation += new CertificateValidationEventHandler(CertificateValidator_CertificateValidation);

			// update security information
			await ApplicationConfiguration.CertificateValidator.Update(ApplicationConfiguration.SecurityConfiguration);
			#region other configuration

			// remove issuer and trusted certificates with the given thumbprints
			if (ThumbprintsToRemove?.Count > 0)
			{
				if (!await RemoveCertificatesAsync(ThumbprintsToRemove))
				{
					throw new Exception("Removing certificates failed.");
				}
			}

			// add trusted issuer certificates
			if (IssuerCertificateBase64Strings?.Count > 0 || IssuerCertificateFileNames?.Count > 0)
			{
				if (!await AddCertificatesAsync(IssuerCertificateBase64Strings, IssuerCertificateFileNames, true))
				{
					throw new Exception("Adding trusted issuer certificate(s) failed.");
				}
			}

			// add trusted peer certificates
			if (TrustedCertificateBase64Strings?.Count > 0 || TrustedCertificateFileNames?.Count > 0)
			{
				if (!await AddCertificatesAsync(TrustedCertificateBase64Strings, TrustedCertificateFileNames, false))
				{
					throw new Exception("Adding trusted peer certificate(s) failed.");
				}
			}

			// update CRL if requested
			if (!string.IsNullOrEmpty(CrlBase64String) || !string.IsNullOrEmpty(CrlFileName))
			{
				if (!await UpdateCrlAsync(CrlBase64String, CrlFileName))
				{
					throw new Exception("CRL update failed.");
				}
			}

			// update application certificate if requested or use the existing certificate
			if (!string.IsNullOrEmpty(NewCertificateBase64String) || !string.IsNullOrEmpty(NewCertificateFileName))
			{
				if (!await UpdateApplicationCertificateAsync(NewCertificateBase64String, NewCertificateFileName, CertificatePassword, PrivateKeyBase64String, PrivateKeyFileName))
				{
					throw new Exception("Update/Setting of the application certificate failed.");
				}
			}
			#endregion

			// use existing certificate, if it is there
			X509Certificate2 certificate = null;
			//certificate = await ApplicationConfiguration.SecurityConfiguration.ApplicationCertificate.Find(true);
			//certificate = await this.keyVaultService.GetCertificate(ApplicationConfiguration.ApplicationName);

			// create a self signed certificate if there is none
			if (certificate == null)
			{
				LoggerManager.Logger.Information($"No existing Application certificate found. Create a self-signed Application certificate valid from yesterday for {CertificateFactory.DefaultLifeTime} months,");
				LoggerManager.Logger.Information($"with a {CertificateFactory.DefaultKeySize} bit key and {CertificateFactory.DefaultHashSize} bit hash.");
				certificate = CertificateFactory.CreateCertificate(
								ApplicationConfiguration.SecurityConfiguration.ApplicationCertificate.StoreType,
								ApplicationConfiguration.SecurityConfiguration.ApplicationCertificate.StorePath,
								null,
								ApplicationConfiguration.ApplicationUri,
								ApplicationConfiguration.ApplicationName,
								ApplicationConfiguration.ApplicationName,
								null,
								CertificateFactory.DefaultKeySize,
								DateTime.UtcNow - TimeSpan.FromDays(1),
								CertificateFactory.DefaultLifeTime,
								CertificateFactory.DefaultHashSize,
								false,
								null,
								null
								);

				//await this.keyVaultService.StoreCertificateAsync(ApplicationConfiguration.ApplicationName, certificate);

				LoggerManager.Logger.Information($"Application certificate with thumbprint '{certificate.Thumbprint}' created.");

				ApplicationConfiguration.SecurityConfiguration.ApplicationCertificate.Certificate = certificate ?? throw new Exception("OPC UA application certificate can not be created! Cannot continue without it!");
				await ApplicationConfiguration.CertificateValidator.UpdateCertificate(ApplicationConfiguration.SecurityConfiguration);
			}
			else
			{
				LoggerManager.Logger.Information($"Application certificate with thumbprint '{certificate.Thumbprint}' found in the application certificate store.");
			}

			//// update security information

			//ApplicationConfiguration.ApplicationUri = GetApplicationUriFromCertificate(certificate);
			LoggerManager.Logger.Information($"Application certificate is for ApplicationUri '{ApplicationConfiguration.ApplicationUri}', ApplicationName '{ApplicationConfiguration.ApplicationName}' and Subject is '{ApplicationConfiguration.ApplicationName}'");

			// we make the default reference stack behavior configurable to put our own certificate into the trusted peer store, but only for self-signed certs
			// note: SecurityConfiguration.AddAppCertToTrustedStore only works for Application instance objects, which we do not have
			if (TrustMyself)
			{
				// ensure it is trusted
				try
				{
					using ICertificateStore trustedStore = ApplicationConfiguration.SecurityConfiguration.TrustedPeerCertificates.OpenStore();
					LoggerManager.Logger.Information($"Adding server certificate to trusted peer store. StorePath={ApplicationConfiguration.SecurityConfiguration.TrustedPeerCertificates.StorePath}");
					await trustedStore.Add(certificate);
				}
				catch (Exception e)
				{
					LoggerManager.Logger.Warning(e, $"Can not add server certificate to trusted peer store. Maybe it is already there.");
				}
			}

			// show CreateSigningRequest data
			if (ShowCreateSigningRequestInfo)
			{
				await ShowCreateSigningRequestInformationAsync(certificate);
			}
		}

		/// <summary>
		/// Show information needed for the Create Signing Request process.
		/// </summary>
		public async Task ShowCreateSigningRequestInformationAsync(X509Certificate2 certificate)
		{
			try
			{
				// we need a certificate with a private key
				if (!certificate.HasPrivateKey)
				{
					// fetch the certificate with the private key
					try
					{
						certificate = await ApplicationConfiguration.SecurityConfiguration.ApplicationCertificate.LoadPrivateKey(null);
					}
					catch (Exception e)
					{
						LoggerManager.Logger.Error(e, $"Error while loading private key.");
						return;
					}
				}
				byte[] certificateSigningRequest = null;
				try
				{
					certificateSigningRequest = CertificateFactory.CreateSigningRequest(certificate);
				}
				catch (Exception e)
				{
					LoggerManager.Logger.Error(e, $"Error while creating signing request.");
					return;
				}
				LoggerManager.Logger.Information($"----------------------- CreateSigningRequest information ------------------");
				LoggerManager.Logger.Information($"ApplicationUri: {ApplicationConfiguration.ApplicationUri}");
				LoggerManager.Logger.Information($"ApplicationName: {ApplicationConfiguration.ApplicationName}");
				LoggerManager.Logger.Information($"ApplicationType: {ApplicationConfiguration.ApplicationType}");
				LoggerManager.Logger.Information($"ProductUri: {ApplicationConfiguration.ProductUri}");
				if (ApplicationConfiguration.ApplicationType != ApplicationType.Client)
				{
					int serverNum = 0;
					foreach (var endpoint in ApplicationConfiguration.ServerConfiguration.BaseAddresses)
					{
						LoggerManager.Logger.Information($"DiscoveryUrl[{serverNum++}]: {endpoint}");
					}
					foreach (var endpoint in ApplicationConfiguration.ServerConfiguration.AlternateBaseAddresses)
					{
						LoggerManager.Logger.Information($"DiscoveryUrl[{serverNum++}]: {endpoint}");
					}
					string[] serverCapabilities = ApplicationConfiguration.ServerConfiguration.ServerCapabilities.ToArray();
					LoggerManager.Logger.Information($"ServerCapabilities: {string.Join(", ", serverCapabilities)}");
				}
				LoggerManager.Logger.Information($"CSR (base64 encoded):");
				Console.WriteLine($"{Convert.ToBase64String(certificateSigningRequest)}");
				LoggerManager.Logger.Information($"---------------------------------------------------------------------------");
				try
				{
					await File.WriteAllBytesAsync($"{ApplicationConfiguration.ApplicationName}.csr", certificateSigningRequest);
					LoggerManager.Logger.Information($"Binary CSR written to '{ApplicationConfiguration.ApplicationName}.csr'");
				}
				catch (Exception e)
				{
					LoggerManager.Logger.Error(e, $"Error while writing .csr file.");
				}
			}
			catch (Exception e)
			{
				LoggerManager.Logger.Error(e, "Error in CSR creation");
			}
		}


		/// <summary>
		/// Show all certificates in the certificate stores.
		/// </summary>
		public async Task ShowCertificateStoreInformationAsync()
		{
			// show trusted issuer certs
			try
			{
				using (ICertificateStore certStore = ApplicationConfiguration.SecurityConfiguration.TrustedIssuerCertificates.OpenStore())
				{
					var certs = await certStore.Enumerate();
					int certNum = 1;
					LoggerManager.Logger.Information($"Trusted issuer store contains {certs.Count} certs");
					foreach (var cert in certs)
					{
						LoggerManager.Logger.Information($"{certNum++:D2}: Subject '{cert.Subject}' (thumbprint: {cert.GetCertHashString()})");
					}
					if (certStore.SupportsCRLs)
					{
						var crls = await certStore.EnumerateCRLs();
						int crlNum = 1;
						LoggerManager.Logger.Information($"Trusted issuer store has {crls.Count} CRLs.");
						foreach (var crl in await certStore.EnumerateCRLs())
						{
							LoggerManager.Logger.Information($"{crlNum++:D2}: Issuer '{crl.Issuer}', Next update time '{crl.NextUpdate}'");
						}
					}
				}
			}
			catch (Exception e)
			{
				LoggerManager.Logger.Error(e, "Error while trying to read information from trusted issuer store.");
			}

			// show trusted peer certs
			try
			{
				using (ICertificateStore certStore = ApplicationConfiguration.SecurityConfiguration.TrustedPeerCertificates.OpenStore())
				{
					var certs = await certStore.Enumerate();
					int certNum = 1;
					LoggerManager.Logger.Information($"Trusted peer store contains {certs.Count} certs");
					foreach (var cert in certs)
					{
						LoggerManager.Logger.Information($"{certNum++:D2}: Subject '{cert.Subject}' (thumbprint: {cert.GetCertHashString()})");
					}
					if (certStore.SupportsCRLs)
					{
						var crls = await certStore.EnumerateCRLs();
						int crlNum = 1;
						LoggerManager.Logger.Information($"Trusted peer store has {crls.Count} CRLs.");
						foreach (var crl in await certStore.EnumerateCRLs())
						{
							LoggerManager.Logger.Information($"{crlNum++:D2}: Issuer '{crl.Issuer}', Next update time '{crl.NextUpdate}'");
						}
					}
				}
			}
			catch (Exception e)
			{
				LoggerManager.Logger.Error(e, "Error while trying to read information from trusted peer store.");
			}

			// show rejected peer certs
			try
			{
				using (ICertificateStore certStore = ApplicationConfiguration.SecurityConfiguration.RejectedCertificateStore.OpenStore())
				{
					var certs = await certStore.Enumerate();
					int certNum = 1;
					LoggerManager.Logger.Information($"Rejected certificate store contains {certs.Count} certs");
					foreach (var cert in certs)
					{
						LoggerManager.Logger.Information($"{certNum++:D2}: Subject '{cert.Subject}' (thumbprint: {cert.GetCertHashString()})");
					}
				}
			}
			catch (Exception e)
			{
				LoggerManager.Logger.Error(e, "Error while trying to read information from rejected certificate store.");
			}
		}

		/// <summary>
		/// Event handler to validate certificates.
		/// </summary>
		public void CertificateValidator_CertificateValidation(CertificateValidator validator, CertificateValidationEventArgs e)
		{
			if (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted)
			{
				e.Accept = AutoAcceptCerts;
				if (AutoAcceptCerts)
				{
					LoggerManager.Logger.Information($"Certificate '{e.Certificate.Subject}' will be trusted, because of corresponding command line option.");
				}
				else
				{
					LoggerManager.Logger.Information($"Not trusting OPC application  with the certificate subject '{e.Certificate.Subject}'.");
					LoggerManager.Logger.Information("If you want to trust this certificate, please copy it from the directory:");
					LoggerManager.Logger.Information($"{ApplicationConfiguration.SecurityConfiguration.RejectedCertificateStore.StorePath}/certs");
					LoggerManager.Logger.Information("to the directory:");
					LoggerManager.Logger.Information($"{ApplicationConfiguration.SecurityConfiguration.TrustedPeerCertificates.StorePath}/certs");
					LoggerManager.Logger.Information($"Rejecting certificate for now.");
				}
			}
		}

		/// <summary>
		/// Delete certificates with the given thumbprints from the trusted peer and issuer certifiate store.
		/// </summary>
		private async Task<bool> RemoveCertificatesAsync(List<string> thumbprintsToRemove)
		{
			bool result = true;

			if (thumbprintsToRemove.Count == 0)
			{
				LoggerManager.Logger.Error($"There is no thumbprint specified for certificates to remove. Please check your command line options.");
				return false;
			}

			// search the trusted peer store and remove certificates with a specified thumbprint
			try
			{
				LoggerManager.Logger.Information($"Starting to remove certificate(s) from trusted peer and trusted issuer store.");
				using (ICertificateStore trustedStore = CertificateStoreIdentifier.OpenStore(ApplicationConfiguration.SecurityConfiguration.TrustedPeerCertificates.StorePath))
				{
					foreach (var thumbprint in thumbprintsToRemove)
					{
						var certToRemove = await trustedStore.FindByThumbprint(thumbprint);
						if (certToRemove != null && certToRemove.Count > 0)
						{
							if (await trustedStore.Delete(thumbprint) == false)
							{
								LoggerManager.Logger.Warning($"Failed to remove certificate with thumbprint '{thumbprint}' from the trusted peer store.");
							}
							else
							{
								LoggerManager.Logger.Information($"Removed certificate with thumbprint '{thumbprint}' from the trusted peer store.");
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				LoggerManager.Logger.Error(e, "Error while trying to remove certificate(s) from the trusted peer store.");
				result = false;
			}

			// search the trusted issuer store and remove certificates with a specified thumbprint
			try
			{
				using (ICertificateStore issuerStore = CertificateStoreIdentifier.OpenStore(ApplicationConfiguration.SecurityConfiguration.TrustedIssuerCertificates.StorePath))
				{
					foreach (var thumbprint in thumbprintsToRemove)
					{
						var certToRemove = await issuerStore.FindByThumbprint(thumbprint);
						if (certToRemove != null && certToRemove.Count > 0)
						{
							if (await issuerStore.Delete(thumbprint) == false)
							{
								LoggerManager.Logger.Warning($"Failed to delete certificate with thumbprint '{thumbprint}' from the trusted issuer store.");
							}
							else
							{
								LoggerManager.Logger.Information($"Removed certificate with thumbprint '{thumbprint}' from the trusted issuer store.");
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				LoggerManager.Logger.Error(e, "Error while trying to remove certificate(s) from the trusted issuer store.");
				result = false;
			}
			return result;
		}

		/// <summary>
		/// Validate and add certificates to the trusted issuer or trusted peer store.
		/// </summary>
		private async Task<bool> AddCertificatesAsync(
						List<string> certificateBase64Strings,
						List<string> certificateFileNames,
						bool issuerCertificate = true)
		{
			bool result = true;

			if (certificateBase64Strings?.Count == 0 && certificateFileNames?.Count == 0)
			{
				LoggerManager.Logger.Error($"There is no certificate provided. Please check your command line options.");
				return false;
			}

			LoggerManager.Logger.Information($"Starting to add certificate(s) to the {(issuerCertificate ? "trusted issuer" : "trusted peer")} store.");
			X509Certificate2Collection certificatesToAdd = new();
			try
			{
				// validate the input and build issuer cert collection
				if (certificateFileNames?.Count > 0)
				{
					foreach (var certificateFileName in certificateFileNames)
					{
						var certificate = new X509Certificate2(certificateFileName);
						certificatesToAdd.Add(certificate);
					}
				}
				if (certificateBase64Strings?.Count > 0)
				{
					foreach (var certificateBase64String in certificateBase64Strings)
					{
						byte[] buffer = new byte[certificateBase64String.Length * 3 / 4];
						if (Convert.TryFromBase64String(certificateBase64String, buffer, out int written))
						{
							var certificate = new X509Certificate2(buffer);
							certificatesToAdd.Add(certificate);
						}
						else
						{
							LoggerManager.Logger.Error($"The provided string '{certificateBase64String[..10]}...' is not a valid base64 string.");
							return false;
						}
					}
				}
			}
			catch (Exception e)
			{
				LoggerManager.Logger.Error(e, $"The issuer certificate data is invalid. Please check your command line options.");
				return false;
			}

			// add the certificate to the right store
			if (issuerCertificate)
			{
				try
				{
					using (ICertificateStore issuerStore = CertificateStoreIdentifier.OpenStore(ApplicationConfiguration.SecurityConfiguration.TrustedIssuerCertificates.StorePath))
					{
						foreach (var certificateToAdd in certificatesToAdd)
						{
							try
							{
								await issuerStore.Add(certificateToAdd);
								LoggerManager.Logger.Information($"Certificate '{certificateToAdd.SubjectName.Name}' and thumbprint '{certificateToAdd.Thumbprint}' was added to the trusted issuer store.");
							}
							catch (ArgumentException)
							{
								// ignore error if cert already exists in store
								LoggerManager.Logger.Information($"Certificate '{certificateToAdd.SubjectName.Name}' already exists in trusted issuer store.");
							}
						}
					}
				}
				catch (Exception e)
				{
					LoggerManager.Logger.Error(e, "Error while adding a certificate to the trusted issuer store.");
					result = false;
				}
			}
			else
			{
				try
				{
					using (ICertificateStore trustedStore = CertificateStoreIdentifier.OpenStore(ApplicationConfiguration.SecurityConfiguration.TrustedPeerCertificates.StorePath))
					{
						foreach (var certificateToAdd in certificatesToAdd)
						{
							try
							{
								await trustedStore.Add(certificateToAdd);
								LoggerManager.Logger.Information($"Certificate '{certificateToAdd.SubjectName.Name}' and thumbprint '{certificateToAdd.Thumbprint}' was added to the trusted peer store.");
							}
							catch (ArgumentException)
							{
								// ignore error if cert already exists in store
								LoggerManager.Logger.Information($"Certificate '{certificateToAdd.SubjectName.Name}' already exists in trusted peer store.");
							}
						}
					}
				}
				catch (Exception e)
				{
					LoggerManager.Logger.Error(e, "Error while adding a certificate to the trusted peer store.");
					result = false;
				}
			}
			return result;
		}

		/// <summary>
		/// Update the CRL in the corresponding store.
		/// </summary>
		private async Task<bool> UpdateCrlAsync(string newCrlBase64String, string newCrlFileName)
		{
			bool result = true;

			if (string.IsNullOrEmpty(newCrlBase64String) && string.IsNullOrEmpty(newCrlFileName))
			{
				LoggerManager.Logger.Error($"There is no CRL specified. Please check your command line options.");
				return false;
			}

			// validate input and create the new CRL
			LoggerManager.Logger.Information($"Starting to update the current CRL.");
			X509CRL newCrl;
			try
			{
				if (string.IsNullOrEmpty(newCrlFileName))
				{
					byte[] buffer = new byte[newCrlBase64String.Length * 3 / 4];
					if (Convert.TryFromBase64String(newCrlBase64String, buffer, out int written))
					{
						newCrl = new X509CRL(buffer);
					}
					else
					{
						LoggerManager.Logger.Error($"The provided string '{newCrlBase64String.Substring(0, 10)}...' is not a valid base64 string.");
						return false;
					}
				}
				else
				{
					newCrl = new X509CRL(newCrlFileName);
				}
			}
			catch (Exception e)
			{
				LoggerManager.Logger.Error(e, $"The new CRL data is invalid.");
				return false;
			}

			// check if CRL was signed by a trusted peer cert
			using (ICertificateStore trustedStore = CertificateStoreIdentifier.OpenStore(ApplicationConfiguration.SecurityConfiguration.TrustedPeerCertificates.StorePath))
			{
				bool trustedCrlIssuer = false;
				var trustedCertificates = await trustedStore.Enumerate();
				foreach (var trustedCertificate in trustedCertificates)
				{
					try
					{
						if (CompareDistinguishedName(newCrl.Issuer, trustedCertificate.Subject) && newCrl.VerifySignature(trustedCertificate, false))
						{
							// the issuer of the new CRL is trusted. delete the crls of the issuer in the trusted store
							LoggerManager.Logger.Information($"Remove the current CRL from the trusted peer store.");
							trustedCrlIssuer = true;
							var crlsToRemove = await trustedStore.EnumerateCRLs(trustedCertificate);
							foreach (var crlToRemove in crlsToRemove)
							{
								try
								{
									if (await trustedStore.DeleteCRL(crlToRemove) == false)
									{
										LoggerManager.Logger.Warning($"Failed to remove CRL issued by '{crlToRemove.Issuer}' from the trusted peer store.");
									}
								}
								catch (Exception e)
								{
									LoggerManager.Logger.Error(e, $"Error while removing the current CRL issued by '{crlToRemove.Issuer}' from the trusted peer store.");
									result = false;
								}
							}
						}
					}
					catch (Exception e)
					{
						LoggerManager.Logger.Error(e, $"Error while removing the cureent CRL from the trusted peer store.");
						result = false;
					}
				}
				// add the CRL if we trust the issuer
				if (trustedCrlIssuer)
				{
					try
					{
						await trustedStore.AddCRL(newCrl);
						LoggerManager.Logger.Information($"The new CRL issued by '{newCrl.Issuer}' was added to the trusted peer store.");
					}
					catch (Exception e)
					{
						LoggerManager.Logger.Error(e, $"Error while adding the new CRL to the trusted peer store.");
						result = false;
					}
				}
			}

			// check if CRL was signed by a trusted issuer cert
			using (ICertificateStore issuerStore = CertificateStoreIdentifier.OpenStore(ApplicationConfiguration.SecurityConfiguration.TrustedIssuerCertificates.StorePath))
			{
				bool trustedCrlIssuer = false;
				var issuerCertificates = await issuerStore.Enumerate();
				foreach (var issuerCertificate in issuerCertificates)
				{
					try
					{
						if (CompareDistinguishedName(newCrl.Issuer, issuerCertificate.Subject) && newCrl.VerifySignature(issuerCertificate, false))
						{
							// the issuer of the new CRL is trusted. delete the crls of the issuer in the trusted store
							LoggerManager.Logger.Information($"Remove the current CRL from the trusted issuer store.");
							trustedCrlIssuer = true;
							var crlsToRemove = await issuerStore.EnumerateCRLs(issuerCertificate);
							foreach (var crlToRemove in crlsToRemove)
							{
								try
								{
									if (await issuerStore.DeleteCRL(crlToRemove) == false)
									{
										LoggerManager.Logger.Warning($"Failed to remove the current CRL issued by '{crlToRemove.Issuer}' from the trusted issuer store.");
									}
								}
								catch (Exception e)
								{
									LoggerManager.Logger.Error(e, $"Error while removing the current CRL issued by '{crlToRemove.Issuer}' from the trusted issuer store.");
									result = false;
								}
							}
						}
					}
					catch (Exception e)
					{
						LoggerManager.Logger.Error(e, $"Error while removing the current CRL from the trusted issuer store.");
						result = false;
					}
				}

				// add the CRL if we trust the issuer
				if (trustedCrlIssuer)
				{
					try
					{
						await issuerStore.AddCRL(newCrl);
						LoggerManager.Logger.Information($"The new CRL issued by '{newCrl.Issuer}' was added to the trusted issuer store.");
					}
					catch (Exception e)
					{
						LoggerManager.Logger.Error(e, $"Error while adding the new CRL issued by '{newCrl.Issuer}' to the trusted issuer store.");
						result = false;
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Validate and update the application.
		/// </summary>
		private async Task<bool> UpdateApplicationCertificateAsync(
						string newCertificateBase64String,
						string newCertificateFileName,
						string certificatePassword,
						string privateKeyBase64String,
						string privateKeyFileName)
		{
			if (string.IsNullOrEmpty(newCertificateFileName) && string.IsNullOrEmpty(newCertificateBase64String))
			{
				LoggerManager.Logger.Error($"There is no new application certificate data provided. Please check your command line options.");
				return false;
			}

			// validate input and create the new application cert
			X509Certificate2 newCertificate;
			try
			{
				if (string.IsNullOrEmpty(newCertificateFileName))
				{
					byte[] buffer = new byte[newCertificateBase64String.Length * 3 / 4];
					if (Convert.TryFromBase64String(newCertificateBase64String, buffer, out int written))
					{
						newCertificate = new X509Certificate2(buffer);
					}
					else
					{
						LoggerManager.Logger.Error($"The provided string '{newCertificateBase64String.Substring(0, 10)}...' is not a valid base64 string.");
						return false;
					}
				}
				else
				{
					newCertificate = new X509Certificate2(newCertificateFileName);
				}
			}
			catch (Exception e)
			{
				LoggerManager.Logger.Error(e, $"The new application certificate data is invalid.");
				return false;
			}

			// validate input and create the private key
			LoggerManager.Logger.Information($"Start updating the current application certificate.");
			byte[] privateKey = null;
			try
			{
				if (!string.IsNullOrEmpty(privateKeyBase64String))
				{
					privateKey = new byte[privateKeyBase64String.Length * 3 / 4];
					if (!Convert.TryFromBase64String(privateKeyBase64String, privateKey, out int written))
					{
						LoggerManager.Logger.Error($"The provided string '{privateKeyBase64String.Substring(0, 10)}...' is not a valid base64 string.");
						return false;
					}
				}
				if (!string.IsNullOrEmpty(privateKeyFileName))
				{
					privateKey = await File.ReadAllBytesAsync(privateKeyFileName);
				}
			}
			catch (Exception e)
			{
				LoggerManager.Logger.Error(e, $"The private key data is invalid.");
				return false;
			}

			// check if there is an application certificate and fetch its data
			bool hasApplicationCertificate = false;
			X509Certificate2 currentApplicationCertificate = null;
			string currentSubjectName = null;
			if (ApplicationConfiguration.SecurityConfiguration.ApplicationCertificate?.Certificate != null)
			{
				hasApplicationCertificate = true;
				currentApplicationCertificate = ApplicationConfiguration.SecurityConfiguration.ApplicationCertificate.Certificate;
				currentSubjectName = currentApplicationCertificate.SubjectName.Name;
				LoggerManager.Logger.Information($"The current application certificate has SubjectName '{currentSubjectName}' and thumbprint '{currentApplicationCertificate.Thumbprint}'.");
			}
			else
			{
				LoggerManager.Logger.Information($"There is no existing application certificate.");
			}

			// for a cert update subject names of current and new certificate must match
			if (hasApplicationCertificate && !CompareDistinguishedName(currentSubjectName, newCertificate.SubjectName.Name))
			{
				LoggerManager.Logger.Error($"The SubjectName '{newCertificate.SubjectName.Name}' of the new certificate doesn't match the current certificates SubjectName '{currentSubjectName}'.");
				return false;
			}

			// if the new cert is not selfsigned verify with the trusted peer and trusted issuer certificates
			try
			{
				if (!CompareDistinguishedName(newCertificate.Subject, newCertificate.Issuer))
				{
					// verify the new certificate was signed by a trusted issuer or trusted peer
					CertificateValidator certValidator = new CertificateValidator();
					CertificateTrustList verificationTrustList = new CertificateTrustList();
					CertificateIdentifierCollection verificationCollection = new CertificateIdentifierCollection();
					using (ICertificateStore issuerStore = CertificateStoreIdentifier.OpenStore(ApplicationConfiguration.SecurityConfiguration.TrustedIssuerCertificates.StorePath))
					{
						var certs = await issuerStore.Enumerate();
						foreach (var cert in certs)
						{
							verificationCollection.Add(new CertificateIdentifier(cert));
						}
					}
					using (ICertificateStore trustedStore = CertificateStoreIdentifier.OpenStore(ApplicationConfiguration.SecurityConfiguration.TrustedPeerCertificates.StorePath))
					{
						var certs = await trustedStore.Enumerate();
						foreach (var cert in certs)
						{
							verificationCollection.Add(new CertificateIdentifier(cert));
						}
					}
					verificationTrustList.TrustedCertificates = verificationCollection;
					certValidator.Update(verificationTrustList, verificationTrustList, null);
					certValidator.Validate(newCertificate);
				}
			}
			catch (Exception e)
			{
				LoggerManager.Logger.Error(e, $"Failed to verify integrity of the new certificate and the trusted issuer list.");
				return false;
			}

			// detect format of new cert and create/update the application certificate
			X509Certificate2 newCertificateWithPrivateKey = null;
			string newCertFormat = null;
			// check if new cert is PFX
			if (string.IsNullOrEmpty(newCertFormat))
			{
				try
				{
					X509Certificate2 certWithPrivateKey = new X509Certificate2(privateKey, certificatePassword);
					newCertificateWithPrivateKey = CertificateFactory.CreateCertificateWithPrivateKey(newCertificate, certWithPrivateKey);
					newCertFormat = "PFX";
					LoggerManager.Logger.Information($"The private key for the new certificate was passed in using PFX format.");
				}
				catch
				{
					LoggerManager.Logger.Debug($"Certificate file is not PFX");
				}
			}
			// check if new cert is PEM
			if (string.IsNullOrEmpty(newCertFormat))
			{
				try
				{
					newCertificateWithPrivateKey = CertificateFactory.CreateCertificateWithPEMPrivateKey(newCertificate, privateKey, certificatePassword);
					newCertFormat = "PEM";
					LoggerManager.Logger.Information($"The private key for the new certificate was passed in using PEM format.");
				}
				catch
				{
					LoggerManager.Logger.Debug($"Certificate file is not PEM");
				}
			}
			if (string.IsNullOrEmpty(newCertFormat))
			{
				// check if new cert is DER and there is an existing application certificate
				try
				{
					if (hasApplicationCertificate)
					{
						X509Certificate2 certWithPrivateKey = await ApplicationConfiguration.SecurityConfiguration.ApplicationCertificate.LoadPrivateKey(certificatePassword);
						newCertificateWithPrivateKey = CertificateFactory.CreateCertificateWithPrivateKey(newCertificate, certWithPrivateKey);
						newCertFormat = "DER";
					}
					else
					{
						LoggerManager.Logger.Error($"There is no existing application certificate we can use to extract the private key. You need to pass in a private key using PFX or PEM format.");
					}
				}
				catch
				{
					LoggerManager.Logger.Debug($"Application certificate format is not DER");
				}
			}

			// if there is no current application cert, we need a new cert with a private key
			if (hasApplicationCertificate)
			{
				if (string.IsNullOrEmpty(newCertFormat))
				{
					LoggerManager.Logger.Error($"The provided format of the private key is not supported (must be PEM or PFX) or the provided cert password is wrong.");
					return false;
				}
			}
			else
			{
				if (string.IsNullOrEmpty(newCertFormat))
				{
					LoggerManager.Logger.Error($"There is no application certificate we can update and for the new application certificate there was not usable private key (must be PEM or PFX format) provided or the provided cert password is wrong.");
					return false;
				}
			}

			// remove the existing and add the new application cert
			using (ICertificateStore appStore = CertificateStoreIdentifier.OpenStore(ApplicationConfiguration.SecurityConfiguration.ApplicationCertificate.StorePath))
			{
				LoggerManager.Logger.Information($"Remove the existing application certificate.");
				try
				{
					if (hasApplicationCertificate && !await appStore.Delete(currentApplicationCertificate.Thumbprint))
					{
						LoggerManager.Logger.Warning($"Removing the existing application certificate with thumbprint '{currentApplicationCertificate.Thumbprint}' failed.");
					}
				}
				catch
				{
					LoggerManager.Logger.Warning($"Failed to remove the existing application certificate from the ApplicationCertificate store.");
				}
				try
				{
					await appStore.Add(newCertificateWithPrivateKey);
					LoggerManager.Logger.Information($"The new application certificate '{newCertificateWithPrivateKey.SubjectName.Name}' and thumbprint '{newCertificateWithPrivateKey.Thumbprint}' was added to the application certificate store.");
				}
				catch (Exception e)
				{
					LoggerManager.Logger.Error(e, $"Failed to add the new application certificate to the application certificate store.");
					return false;
				}
			}

			// update the application certificate
			try
			{
				LoggerManager.Logger.Information($"Activating the new application certificate with thumbprint '{newCertificateWithPrivateKey.Thumbprint}'.");
				ApplicationConfiguration.SecurityConfiguration.ApplicationCertificate.Certificate = newCertificate;
				await ApplicationConfiguration.CertificateValidator.UpdateCertificate(ApplicationConfiguration.SecurityConfiguration);
			}
			catch (Exception e)
			{
				LoggerManager.Logger.Error(e, $"Failed to activate the new application certificate.");
				return false;
			}

			return true;
		}

		public bool CompareDistinguishedName(string dn1, string dn2)
		{
			var name1 = new X500DistinguishedName(dn1).Name;
			var name2 = new X500DistinguishedName(dn2).Name;

			return string.Equals(name1, name2, StringComparison.OrdinalIgnoreCase);
		}
	}
}
