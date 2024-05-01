namespace QIA.Opc.Infrastructure.Managers;

using System.Collections.Concurrent;
using System.Net.NetworkInformation;
using global::Opc.Ua;
using global::Opc.Ua.Client;
using MediatR;
using Qia.Opc.Domain.Entities.Enums;
using Qia.Opc.OPCUA.Connector;
using QIA.Opc.Application.Events;
using QIA.Opc.Application.Requests;
using QIA.Opc.Infrastructure.Entities;
using static QIA.Opc.Infrastructure.Application.LoggerManager;

/// <summary>
/// Sessions container
/// </summary>
public class SessionManager
{
    private readonly ApplicationConfiguration _applicationConfiguration;
    private readonly IPublisher _publisher;
    private readonly ConcurrentDictionary<string, OPCUASession> _sessions = new();
    private readonly NamespaceTable _namespaceTable;
    private double minSupportedSamplingInterval;

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
    private uint missedKeepAlives;
    /// <summary>
    /// Number of missed keep alives allowed, before disconnecting the session.
    /// </summary>
    public static uint OpcKeepAliveDisconnectThreshold { get; set; } = 10;
    /// <summary>
    /// Number of unsuccessful connection attempts.
    /// </summary>
    private uint unsuccessfulConnectionCount;

    public SessionManager(ApplicationConfiguration applicationConfiguration, IPublisher publisher)
    {
        _applicationConfiguration = applicationConfiguration;
        _publisher = publisher;
        _namespaceTable = new NamespaceTable();
    }

    public IEnumerable<OPCUASession> GetSessionList()
    {
        List<OPCUASession> sessions = _sessions.Values.ToList();

        return sessions;
    }

    public void TryGetSession(string sessionNodeId, out OPCUASession session, bool isConnecting = false)
    {
        session = null;
        if (string.IsNullOrWhiteSpace(sessionNodeId))
        {
            return;
        }

        _sessions.TryGetValue(sessionNodeId, out session);
        if (session == null && !isConnecting)
        {
            throw new Exception("Session is not connected");
        }
        else
        {
            //session.LastAccessed = DateTime.UtcNow;
        }
    }

    public OPCUASession CreateUniqueSession(SessionRequest sessionRequest)
    {
        EndpointDescription selectedEndpoint = null;
        ConfiguredEndpoint configuredEndpoint = null;

        OPCUASession newSession = new()
        {
            EndpointUrl = sessionRequest.EndpointUrl,
            Name = sessionRequest.Name,
            Guid = sessionRequest.Guid,
        };

        try
        {

            CheckIfMachineIsActive(newSession.EndpointUrl);

            selectedEndpoint = CoreClientUtils.SelectEndpoint(_applicationConfiguration, newSession.EndpointUrl, useSecurity: ApplicationConfigBuilder.UseSecurity);

            configuredEndpoint = new ConfiguredEndpoint(null, selectedEndpoint, EndpointConfiguration.Create(_applicationConfiguration));

            Logger.Information($"Create {(ApplicationConfigBuilder.UseSecurity ? "secured" : "unsecured")} session for endpoint URI: '{newSession.EndpointUrl}' name: '{newSession.Name}' with timeout of {newSession.ExpiryDuration} h.");

            // Create an OPC UA session with the selected endpoint.
            Session session = Session.Create(
                _applicationConfiguration,
                configuredEndpoint,
                false,
                "",
                sessionTimeout: (uint)newSession.ExpiryDuration.Milliseconds,
                new UserIdentity(new AnonymousIdentityToken()),
                null).GetAwaiter().GetResult();

            newSession = new OPCUASession
            {
                EndpointUrl = sessionRequest.EndpointUrl,
                Name = sessionRequest.Name,
                Guid = sessionRequest.Guid,
                SessionNodeId = session.SessionId.ToString(),
                CreatedAt = DateTime.UtcNow,
                LastAccessed = DateTime.UtcNow,
                State = SessionState.Connected,
                Session = session
            };

            _sessions.TryAdd(session.SessionId.ToString(), newSession);
            return newSession;
        }
        catch (ServiceResultException ex) when (ex.StatusCode == StatusCodes.BadCertificateTimeInvalid)
        {
            Logger.Error(ex, "The certificate is not valid as per the current system time. Check the certificate validity period and system clock.");
            throw;
        }
        catch (Exception e)
        {
            Logger.Error(e, $"Session creation to endpoint '{newSession.EndpointUrl}' failed {++unsuccessfulConnectionCount} time(s). Please verify if server is up and configuration is correct.\n {e.Message} {e.InnerException?.Message} {e.StackTrace}");

            RemoveSession(newSession.SessionNodeId);
            throw;
        }
        finally
        {
            if (newSession.Session != null)
            {

                Logger.Information($"Session successfully created with Id {newSession.Session.SessionId}.");
                if (!selectedEndpoint.EndpointUrl.Equals(configuredEndpoint.EndpointUrl.AbsoluteUri, StringComparison.OrdinalIgnoreCase))
                {
                    Logger.Information($"the Server has updated the EndpointUrl to '{selectedEndpoint.EndpointUrl}'");
                }

                newSession.Session.KeepAliveInterval = OpcKeepAliveInterval * 1000;
                newSession.Session.KeepAlive += StandardClient_KeepAlive;

                // fetch the namespace array and cache it. it will not change as long the session exists.
                DataValue namespaceArrayNodeValue = newSession.Session.ReadValue(VariableIds.Server_NamespaceArray);
                _namespaceTable.Update(namespaceArrayNodeValue.GetValue<string[]>(null));

                // show the available namespaces
                Logger.Information($"The session to endpoint '{selectedEndpoint.EndpointUrl}' has {_namespaceTable.Count} entries in its namespace array:");
                var i = 0;
                foreach (var ns in _namespaceTable.ToArray())
                {
                    Logger.Information($"Namespace index {i++}: {ns}");
                }

                // fetch the minimum supported item sampling interval from the server.
                DataValue minSupportedSamplingInterval = newSession.Session.ReadValue(VariableIds.Server_ServerCapabilities_MinSupportedSampleRate);
                this.minSupportedSamplingInterval = minSupportedSamplingInterval.GetValue(0);
                Logger.Information($"The server on endpoint '{selectedEndpoint.EndpointUrl}' supports a minimal sampling interval of {this.minSupportedSamplingInterval} ms.");
                newSession.State = SessionState.Connected;
            }
            else
            {
                newSession.State = SessionState.Disconnected;
            }
        }
    }

    /// <summary>
    /// Disconnects a session and removes all subscriptions on it and marks all nodes on those subscriptions
    /// as unmonitored.
    /// </summary>
    public void DisconnectSession(string sessionNodeId)
    {
        if (string.IsNullOrWhiteSpace(sessionNodeId))
        {
            return;
        }

        TryGetSession(sessionNodeId, out OPCUASession session);
        if (session == null)
        {
            return;
        }

        try
        {
            Logger.Information($"Closing session to endpoint URI '{session.EndpointUrl}' closed successfully.");

            session.Session.Close();
            Logger.Information($"Session to endpoint URI '{session.EndpointUrl}' closed successfully.");

            RemoveSession(session.SessionNodeId);
            missedKeepAlives = 0;
        }
        catch (Exception ex)
        {
            Logger.Error($"Error on session close '{session.EndpointUrl}' {ex.Message}");
            throw;
        }
    }

    private void RemoveSession(string sessionNodeId)
    {
        try
        {
            var result = _sessions.TryRemove(sessionNodeId, out _);
        }
        catch
        {
            //session is removed already
        }
    }

    private static void CheckIfMachineIsActive(string endpointUrl)
    {
        Uri url = new(endpointUrl);
        Ping pinger = new();
        PingReply reply = pinger.Send(url.Host);
        if (reply.Status != IPStatus.Success)
        {
            Logger.Error($"PING {url.Host}: {reply.Status}");
            throw new Exception($"Connection the the IP {url.Host} failed");
        }

        Logger.Information($"PING {url.Host}: {reply.Status}");
    }

    /// <summary>
    /// Handler for the standard "keep alive" event sent by all OPC UA servers.
    /// </summary>
    private void StandardClient_KeepAlive(ISession session, KeepAliveEventArgs e)
    {
        try
        {
            if (e != null && session != null && session.ConfiguredEndpoint != null)
            {
                if (!ServiceResult.IsGood(e.Status))
                {
                    Logger.Warning($"Session endpoint: {session.ConfiguredEndpoint.EndpointUrl} has Status: {e.Status}");
                    Logger.Information($"Outstanding requests: {session.OutstandingRequestCount}, Defunct requests: {session.DefunctRequestCount}");
                    Logger.Information($"Good publish requests: {session.GoodPublishRequestCount}, KeepAlive interval: {session.KeepAliveInterval}");
                    Logger.Information($"SessionId: {session.SessionId}");

                    missedKeepAlives++;
                    Logger.Information($"Missed KeepAlives: {missedKeepAlives}");

                    if (missedKeepAlives >= OpcKeepAliveDisconnectThreshold)
                    {
                        Logger.Warning($"Hit configured missed keep alive threshold of {OpcKeepAliveDisconnectThreshold}. Disconnecting the session to endpoint {session.ConfiguredEndpoint.EndpointUrl}.");
                        session.KeepAlive -= StandardClient_KeepAlive;


                        SessionStateEvent sessionStateEvent = new()
                        {
                            Message = "Session is: " + e.Status.ToString(),
                            Title = "Session state",
                            SessionId = session?.SessionId?.ToString(),
                            LogCategory = LogCategory.Warning
                        };

                        _publisher.Publish(sessionStateEvent);

                        if (session.SessionId == null)
                        {
                            return;
                        }

                        _sessions.TryRemove(session.SessionId.ToString(), out _);
                        session.CloseAsync().GetAwaiter().GetResult();
                    }
                }
                else
                {
                    if (missedKeepAlives != 0)
                    {
                        // reset missed keep alive count
                        Logger.Information($"Session endpoint: {session.ConfiguredEndpoint.EndpointUrl} got a keep alive after {missedKeepAlives} {(missedKeepAlives == 1 ? "was" : "were")} missed.");
                        missedKeepAlives = 0;
                    }
                }
            }
            else
            {
                Logger.Warning("Keep alive arguments seems to be wrong.");
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"Error in keep alive handling for endpoint '{session.ConfiguredEndpoint.EndpointUrl}'. (message: '{ex.Message}'");
        }
    }
}
