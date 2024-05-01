namespace QIA.Opc.Infrastructure.Services.OPCUA;

using System.Net;
using AutoMapper;
using Qia.Opc.Domain.Entities;
using Qia.Opc.Domain.Entities.Enums;
using QIA.Opc.Application.Requests;
using QIA.Opc.Application.Responses;
using QIA.Opc.Domain.Repositories;
using QIA.Opc.Infrastructure.Application;
using QIA.Opc.Infrastructure.Managers;

/// <summary>
/// session management
/// </summary>
public class SessionService
{
    private readonly SessionManager _sessionManager;
    private readonly IMapper _mapper;
    private readonly IGenericRepository<SessionConfig> _sessionRepo;

    public SessionService(SessionManager sessionManager,
                    IMapper mapper,
                    IGenericRepository<SessionConfig> sessionRepo)
    {
        _sessionManager = sessionManager;
        _mapper = mapper;
        _sessionRepo = sessionRepo;
    }

    public async Task<ApiResponse<IEnumerable<SessionResponse>>> GetSessionsAsync()
    {
        IEnumerable<SessionConfig> savedSessionsResult = await _sessionRepo.ListAllAsync();
        IEnumerable<Entities.OPCUASession> activeSessionsResult = _sessionManager.GetSessionList();

        IEnumerable<SessionResponse> savedSessions = _mapper.Map<IEnumerable<SessionResponse>>(savedSessionsResult);
        IEnumerable<SessionResponse> activeSessions = _mapper.Map<IEnumerable<SessionResponse>>(activeSessionsResult);

        // Merging the sessions
        IEnumerable<SessionResponse> mergedSessions = savedSessions
            .GroupJoin(activeSessions,
                saved => saved.Guid,
                active => active.Guid,
                (saved, activeGroup) => new { Saved = saved, Active = activeGroup.FirstOrDefault() })
            .Select(m => m.Active ?? m.Saved);

        return ApiResponse<IEnumerable<SessionResponse>>.Success(mergedSessions);
    }

    public async Task<ApiResponse<SessionResponse>> CreateEndpointAsync(SessionRequest sessionRequest)
    {
        SessionConfig sessionEntity = new()
        {
            CreatedAt = DateTime.UtcNow,
            Name = sessionRequest.Name,
            EndpointUrl = sessionRequest.EndpointUrl,
            Guid = Guid.NewGuid().ToString(),
        };

        await _sessionRepo.AddAsync(sessionEntity);

        SessionResponse sessionResponse = _mapper.Map<SessionResponse>(sessionEntity);

        return ApiResponse<SessionResponse>.Success(sessionResponse, HttpStatusCode.Created);
    }

    public ApiResponse<SessionResponse> GetSessionIfActive(string sessionNodeId)
    {
        try
        {
            _sessionManager.TryGetSession(sessionNodeId, out Entities.OPCUASession opcUaSession, true);
            // if not found - create new 
            if (opcUaSession == null)
            {
                return ApiResponse<SessionResponse>.Success(null, HttpStatusCode.NotFound);
            }

            SessionResponse sessionResponse = _mapper.Map<SessionResponse>(opcUaSession);

            if (sessionResponse.State == SessionState.Connected)
            {
                return ApiResponse<SessionResponse>.Success(sessionResponse);
            }
            else
            {
                _sessionManager.DisconnectSession(sessionNodeId);
            }

            return ApiResponse<SessionResponse>.Failure(HttpStatusCode.NotFound, "Active session not found, will create a new one");
        }
        catch (Exception)
        {
            throw;
        }
    }

    public ApiResponse<SessionResponse> CreateUniqueSessionAsync(SessionRequest sessionRequest)
    {
        try
        {
            Entities.OPCUASession session = _sessionManager.CreateUniqueSession(sessionRequest);
            if (session.State == SessionState.Disconnected)
            {
                return ApiResponse<SessionResponse>.Failure(HttpStatusCode.BadRequest, $"{sessionRequest.Name} {sessionRequest.EndpointUrl}");
            }

            SessionResponse sessionResponse = _mapper.Map<SessionResponse>(session);

            //var sessionEntity = await sessionRepo.FindAsync(c => c.Id == sessionId);
            //sessionEntity.LastAccessed = DateTime.UtcNow;
            //await sessionRepo.UpsertAsync(sessionEntity, c => c.Id == sessionId);

            return ApiResponse<SessionResponse>.Success(sessionResponse, HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            return ApiResponse<SessionResponse>.Failure(HttpStatusCode.BadGateway, ex.Message);
        }
    }

    public async Task<ApiResponse<bool>> DeleteSessionAsync(string sessionGuid)
    {
        SessionConfig sessionToDelete = await _sessionRepo.FindAsync(c => c.Guid == sessionGuid);

        if (sessionToDelete == null)
        {
            return ApiResponse<bool>.Failure(HttpStatusCode.NotFound);
        }

        await _sessionRepo.DeleteAsync(sessionToDelete);

        _sessionManager.DisconnectSession(sessionGuid.ToString());
        return ApiResponse<bool>.Success(true);
    }

    public async Task<ApiResponse<SessionConfig>> UpdateEndpointAsync(SessionRequest request)
    {
        // check if editing connected session
        _sessionManager.TryGetSession(request.SessionNodeId, out Entities.OPCUASession session, true);

        if (session != null && session.State == SessionState.Connected)
        {
            return ApiResponse<SessionConfig>.Failure(HttpStatusCode.Forbidden, "Cannot edit connected session");
        }

        // update database
        SessionConfig sessionEntity = await _sessionRepo.FindAsync(c => c.Guid == request.Guid);
        sessionEntity.Name = request.Name;
        sessionEntity.EndpointUrl = request.EndpointUrl;
        await _sessionRepo.UpsertAsync(sessionEntity, c => c.Guid == request.Guid);

        return ApiResponse<SessionConfig>.Success(sessionEntity);
    }

    public ApiResponse<bool> Disconnect(string sessionNodeId)
    {
        _sessionManager.DisconnectSession(sessionNodeId);

        return ApiResponse<bool>.Success(true);
    }
}
