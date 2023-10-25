using AutoMapper;
using Opc.Ua;
using Opc.Ua.Client;
using Qia.Opc.Domain.Common;
using Qia.Opc.Domain.DTO;
using Qia.Opc.Domain.Entities.Enums;
using Qia.Opc.OPCUA.Connector.Entities;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace Qia.Opc.OPCUA.Connector.Managers
{
	using static Qia.Opc.Domain.Core.LoggerManager;

	/// <summary>
	/// Sessions container
	/// </summary>
	public class SessionManager
	{
		private readonly ApplicationConfiguration _applicationConfiguration;
		private readonly IMapper mapper;
		private ConcurrentDictionary<string, OPCUASession> _sessions = new ConcurrentDictionary<string, OPCUASession>();
		private OPCUASession _currentSession;
		private SessionReconnectHandler m_reconnectHandler;
		public OPCUASession CurrentSession => _currentSession ?? new OPCUASession();

		/// <summary>
		/// Max timeout when creating a new session to a server.
		/// </summary>
		public static uint OpcSessionCreationTimeout { get; set; } = 10;
		/// <summary>
		/// Keep alive interval.
		/// </summary>
		public static int OpcKeepAliveInterval { get; set; } = 2;
		/// <summary>
		/// Number of missed keep alives.
		/// </summary>
		public uint MissedKeepAlives;
		/// <summary>
		/// Number of missed keep alives allowed, before disconnecting the session.
		/// </summary>
		public static uint OpcKeepAliveDisconnectThreshold { get; set; } = 10;
		/// <summary>
		/// Number of unsuccessful connection attempts.
		/// </summary>
		public uint UnsuccessfulConnectionCount;

		public delegate void EventMessageHandler(object sender, EventData e);
		public event EventMessageHandler EventMessage;

		public SessionManager(ApplicationConfiguration applicationConfiguration, IMapper mapper)
		{
			this.mapper = mapper;
			_applicationConfiguration = applicationConfiguration;
			_namespaceTable = new NamespaceTable();

		}

		public OPCUASession CreateUniqueSession(SessionDTO sessionDto)
		{
			//reconnect
			if (_currentSession != null && _currentSession.Name == sessionDto.Name && _currentSession.EndpointUrl == sessionDto.EndpointUrl)
			{
				RemoveSession(_currentSession.SessionId);
			}

			_currentSession = new OPCUASession();
			EndpointDescription selectedEndpoint = null;
			ConfiguredEndpoint configuredEndpoint = null;
			try
			{
				_currentSession.EndpointUrl = sessionDto.EndpointUrl;

				// release the session to not block for high network timeouts

				// Select the endpoint based on given URL and security preference.
				selectedEndpoint = CoreClientUtils.SelectEndpoint(_applicationConfiguration, _currentSession.EndpointUrl, useSecurity: ApplicationConfigBuilder.UseSecurity);

				configuredEndpoint = new ConfiguredEndpoint(null, selectedEndpoint, EndpointConfiguration.Create(_applicationConfiguration));

				Logger.Information($"Create {(ApplicationConfigBuilder.UseSecurity ? "secured" : "unsecured")} session for endpoint URI: '{_currentSession.EndpointUrl}' name: '{sessionDto.Name}' with timeout of {_currentSession.ExpiryDuration} h.");

				// Create an OPC UA session with the selected endpoint.
				var session = global::Opc.Ua.Client.Session.Create(
					_applicationConfiguration,
					configuredEndpoint,
					false,
					"",
					sessionTimeout: (uint)_currentSession.ExpiryDuration.Milliseconds,
					new UserIdentity(new AnonymousIdentityToken()),
					null).GetAwaiter().GetResult();


				var uniqueId = Guid.NewGuid().ToString();
				_currentSession = new OPCUASession
				{
					Name = sessionDto.Name,
					SessionId = uniqueId,
					EndpointUrl = sessionDto.EndpointUrl,
					CreatedAt = DateTime.UtcNow,
					LastAccessed = DateTime.UtcNow,
					State = SessionState.Connected,
					Session = session
				};

				_sessions.TryAdd(uniqueId, _currentSession);
				return _currentSession;
			}
			catch (Exception e)
			{
				Logger.Error(e, $"Session creation to endpoint '{_currentSession.EndpointUrl}' failed {++UnsuccessfulConnectionCount} time(s). Please verify if server is up and configuration is correct.");

				RemoveSession(_currentSession.SessionId);
				return _currentSession;
			}
			finally
			{
				if (_currentSession.Session != null)
				{

					Logger.Information($"Session successfully created with Id {_currentSession.Session.SessionId}.");
					if (!selectedEndpoint.EndpointUrl.Equals(configuredEndpoint.EndpointUrl.AbsoluteUri, StringComparison.OrdinalIgnoreCase))
					{
						Logger.Information($"the Server has updated the EndpointUrl to '{selectedEndpoint.EndpointUrl}'");
					}

					_currentSession.Session.KeepAliveInterval = OpcKeepAliveInterval * 1000;
					_currentSession.Session.KeepAlive += StandardClient_KeepAlive;

					// fetch the namespace array and cache it. it will not change as long the session exists.
					DataValue namespaceArrayNodeValue = _currentSession.Session.ReadValue(VariableIds.Server_NamespaceArray);
					_namespaceTable.Update(namespaceArrayNodeValue.GetValue<string[]>(null));

					// show the available namespaces
					Logger.Information($"The session to endpoint '{selectedEndpoint.EndpointUrl}' has {_namespaceTable.Count} entries in its namespace array:");
					int i = 0;
					foreach (var ns in _namespaceTable.ToArray())
					{
						Logger.Information($"Namespace index {i++}: {ns}");
					}

					// fetch the minimum supported item sampling interval from the server.
					DataValue minSupportedSamplingInterval = _currentSession.Session.ReadValue(VariableIds.Server_ServerCapabilities_MinSupportedSampleRate);
					_minSupportedSamplingInterval = minSupportedSamplingInterval.GetValue(0);
					Logger.Information($"The server on endpoint '{selectedEndpoint.EndpointUrl}' supports a minimal sampling interval of {_minSupportedSamplingInterval} ms.");
					_currentSession.State = SessionState.Connected;
				}
				else
				{
					_currentSession.State = SessionState.Disconnected;
				}
			}
		}

		public void TryGetSession(string sessionId, out OPCUASession session)
		{
			if (sessionId == null)
			{
				session = null;
				return;
			}
			_sessions.TryGetValue(sessionId, out session);
			if (session != null)
				session.LastAccessed = DateTime.UtcNow;
			_currentSession = session;
		}

		// Cleanup expired sessions based on inactivity
		public void CleanupExpiredSessions(TimeSpan inactivityDuration)
		{
			var expiredSessions = _sessions.Where(s => DateTime.UtcNow - s.Value.LastAccessed > inactivityDuration).Select(s => s.Key).ToList();
			foreach (var sessionId in expiredSessions)
			{
				_sessions.TryRemove(sessionId, out _);
			}
		}

		public bool RemoveSession(string sessionId)
		{
			try
			{
				var result = _sessions.TryRemove(sessionId, out _);
				_currentSession = new OPCUASession();
				return result;
			}
			catch (Exception ex)
			{
				return false;
			}
		}


		/// <summary>
		/// Handler for the standard "keep alive" event sent by all OPC UA servers.
		/// </summary>
		private void StandardClient_KeepAlive(ISession session, KeepAliveEventArgs e)
		{
			if (e != null && session != null && session.ConfiguredEndpoint != null && _currentSession.Session != null)
			{
				try
				{
					if (!ServiceResult.IsGood(e.Status))
					{
						Logger.Warning($"Session endpoint: {session.ConfiguredEndpoint.EndpointUrl} has Status: {e.Status}");
						Logger.Information($"Outstanding requests: {session.OutstandingRequestCount}, Defunct requests: {session.DefunctRequestCount}");
						Logger.Information($"Good publish requests: {session.GoodPublishRequestCount}, KeepAlive interval: {session.KeepAliveInterval}");
						Logger.Information($"SessionId: {session.SessionId}");

						if (_currentSession.State == SessionState.Connected)
						{
							MissedKeepAlives++;
							Logger.Information($"Missed KeepAlives: {MissedKeepAlives}");
							if (MissedKeepAlives >= OpcKeepAliveDisconnectThreshold)
							{
								EventMessage?.Invoke(this, new EventData
								{
									Message = e.Status.ToString(),
									Title = "Session state"
								});
								Logger.Warning($"Hit configured missed keep alive threshold of {OpcKeepAliveDisconnectThreshold}. Disconnecting the session to endpoint {session.ConfiguredEndpoint.EndpointUrl}.");
								session.KeepAlive -= StandardClient_KeepAlive;
								Task t = Task.Run(async () => await DisconnectAsync());
							}
						}
					}
					else
					{
						if (MissedKeepAlives != 0)
						{
							// reset missed keep alive count
							Logger.Information($"Session endpoint: {session.ConfiguredEndpoint.EndpointUrl} got a keep alive after {MissedKeepAlives} {(MissedKeepAlives == 1 ? "was" : "were")} missed.");
							MissedKeepAlives = 0;
						}
					}
				}
				catch (Exception ex)
				{
					Logger.Error(ex, $"Error in keep alive handling for endpoint '{session.ConfiguredEndpoint.EndpointUrl}'. (message: '{ex.Message}'");
				}
			}
			else
			{
				Logger.Warning("Keep alive arguments seems to be wrong.");
			}
		}

		/// <summary>
		/// Disconnects a session and removes all subscriptions on it and marks all nodes on those subscriptions
		/// as unmonitored.
		/// </summary>
		public async Task DisconnectAsync()
		{
			try
			{
				try
				{
					_currentSession.Session.Close();
				}
				catch
				{
					// the session might be already invalidated. ignore
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Error in DisconnectAsyn {0} {1}");
			}
			RemoveSession(_currentSession.SessionId);
			MissedKeepAlives = 0;
			await Task.CompletedTask;
		}
		/// <summary>
		/// Internal disconnect method. Caller must have taken the _opcSessionSemaphore.
		/// </summary>
		private void InternalDisconnect()
		{

		}

		/// <summary>
		/// Shutdown the current session if it is connected.
		/// </summary>
		public void ShutdownAsync()
		{
			bool sessionLocked = false;
			try
			{

				// if the session is connected, close it
				if (sessionLocked && (_currentSession.State == SessionState.Connecting || _currentSession.State == SessionState.Connected))
				{
					try
					{
						Logger.Information($"Closing session to endpoint URI '{_currentSession.EndpointUrl}' closed successfully.");
						_currentSession.Session.Close();
						_currentSession.State = SessionState.Disconnected;
						Logger.Information($"Session to endpoint URI '{_currentSession.EndpointUrl}' closed successfully.");
					}
					catch (Exception e)
					{
						Logger.Error(e, $"Error while closing session to endpoint '{_currentSession.EndpointUrl}'.");
						_currentSession.State = SessionState.Disconnected;
						return;
					}
				}
			}
			finally
			{
				if (sessionLocked)
				{
					// cancel all threads waiting on the session semaphore
					_sessionCancelationTokenSource.Cancel();
				}
			}
		}

		public IEnumerable<Domain.Entities.SessionEntity> GetSessionList()
		{
			var sessions = _sessions.Values.ToList();

			var result = mapper.Map<IEnumerable<Domain.Entities.SessionEntity>>(sessions);
			return result;
		}

		private CancellationTokenSource _sessionCancelationTokenSource;
		private NamespaceTable _namespaceTable;
		private double _minSupportedSamplingInterval;

	}
}
