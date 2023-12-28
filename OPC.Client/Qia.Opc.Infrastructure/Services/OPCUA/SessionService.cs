using AutoMapper;
using Qia.Opc.Domain.Core;
using Qia.Opc.Domain.Entities;
using Qia.Opc.Domain.Entities.Enums;
using Qia.Opc.OPCUA.Connector.Managers;
using QIA.Opc.Domain.Repository;
using QIA.Opc.Domain.Requests;
using QIA.Opc.Domain.Responses;

namespace QIA.Opc.Infrastructure.Services.OPCUA
{
	/// <summary>
	/// session management
	/// </summary>
	public class SessionService
	{
		private readonly SessionManager _sessionManager;
		private readonly IMapper mapper;
		private readonly IGenericRepository<SessionEntity> sessionRepo;

		public SessionService(SessionManager sessionManager,
						IMapper mapper,
						IGenericRepository<SessionEntity> sessionRepo)
		{
			_sessionManager = sessionManager;
			this.mapper = mapper;
			this.sessionRepo = sessionRepo;
		}

		public async Task<ApiResponse<IEnumerable<SessionResponse>>> GetSessionsAsync()
		{
			var savedSessionsResult = await sessionRepo.ListAllAsync();
			var activeSessionsResult = _sessionManager.GetSessionList();

			var savedSessions = mapper.Map<IEnumerable<SessionResponse>>(savedSessionsResult);
			var activeSessions = mapper.Map<IEnumerable<SessionResponse>>(activeSessionsResult);

			// Merging the sessions
			var mergedSessions = savedSessions
				.GroupJoin(activeSessions,
					saved => saved.SessionGuidId,
					active => active.SessionGuidId,
					(saved, activeGroup) => new { Saved = saved, Active = activeGroup.FirstOrDefault() })
				.Select(m => m.Active ?? m.Saved);

			return ApiResponse<IEnumerable<SessionResponse>>.Success(mergedSessions);
		}

		public async Task<ApiResponse<SessionResponse>> CreateEndpointAsync(SessionRequest sessionRequest)
		{
			var sessionEntity = new SessionEntity
			{
				CreatedAt = DateTime.UtcNow,
				Name = sessionRequest.Name,
				EndpointUrl = sessionRequest.EndpointUrl,
				SessionGuidId = Guid.NewGuid().ToString(),
			};

			await sessionRepo.AddAsync(sessionEntity);

			var sessionResponse = mapper.Map<SessionResponse>(sessionEntity);

			return ApiResponse<SessionResponse>.Success(sessionResponse, HttpStatusCode.Created);
		}

		public ApiResponse<SessionResponse> GetSessionIfActive(string sessionNodeId)
		{
			try
			{
				_sessionManager.TryGetSession(sessionNodeId, out var opcUaSession, true);
				// if not found - create new 
				if (opcUaSession == null) return ApiResponse<SessionResponse>.HandledFailure(HttpStatusCode.NotFound, "Create a new session");

				var sessionResponse = mapper.Map<SessionResponse>(opcUaSession);

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
			catch (Exception ex)
			{

				throw;
			}

			
		}

		public ApiResponse<SessionResponse> CreateUniqueSessionAsync(SessionRequest sessionRequest)
		{
			var session = _sessionManager.CreateUniqueSession(sessionRequest);
			if (session.State == SessionState.Disconnected)
			{
				return ApiResponse<SessionResponse>.Failure(HttpStatusCode.BadGateway, $"{sessionRequest.Name} {sessionRequest.EndpointUrl}");
			}

			var sessionResponse = mapper.Map<SessionResponse>(session);

			//var sessionEntity = await sessionRepo.FindAsync(c => c.Id == sessionId);
			//sessionEntity.LastAccessed = DateTime.UtcNow;
			//await sessionRepo.UpsertAsync(sessionEntity, c => c.Id == sessionId);

			return ApiResponse<SessionResponse>.Success(sessionResponse, HttpStatusCode.Created);
		}

		public async Task<ApiResponse<bool>> DeleteSessionAsync(string sessionNodeId)
		{
			var sessionToDelete = await sessionRepo.FindAsync(c => c.SessionGuidId == sessionNodeId);

			if (sessionToDelete == null) return ApiResponse<bool>.Failure(HttpStatusCode.NotFound);
			await sessionRepo.DeleteAsync(sessionToDelete);

			_sessionManager.DisconnectSession(sessionNodeId);
			return ApiResponse<bool>.Success(true);
		}


		public async Task<ApiResponse<SessionEntity>> UpdateEndpointAsync(SessionRequest request)
		{
			// check if editing connected session
			_sessionManager.TryGetSession(request.SessionNodeId, out var session, true);

			if (session != null && session.State == SessionState.Connected)
			{
				return ApiResponse<SessionEntity>.Failure(HttpStatusCode.Forbidden, "Cannot edit connected session");
			}

			// update database
			var sessionEntity = await sessionRepo.FindAsync(c => c.SessionGuidId == request.SessionGuidId);
			sessionEntity.Name = request.Name;
			sessionEntity.EndpointUrl = request.EndpointUrl;
			await sessionRepo.UpsertAsync(sessionEntity, c => c.SessionGuidId == request.SessionGuidId);

			return ApiResponse<SessionEntity>.Success(sessionEntity);
		}


		public ApiResponse<bool> Disconnect(string sessionNodeId)
		{
			_sessionManager.DisconnectSession(sessionNodeId);

			return ApiResponse<bool>.Success(true);
		}
	}
}