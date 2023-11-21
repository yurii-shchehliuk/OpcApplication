using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Qia.Opc.Domain.Core;
using Qia.Opc.Domain.Entities;
using Qia.Opc.Domain.Entities.Enums;
using Qia.Opc.Infrastructure.Application;
using Qia.Opc.OPCUA.Connector.Entities;
using Qia.Opc.OPCUA.Connector.Managers;
using Qia.Opc.Persistence.Repository;
using QIA.Opc.Domain.Request;

namespace QIA.Opc.Infrastructure.Services.OPCUA
{
	/// <summary>
	/// session management
	/// </summary>
	public class SessionService
	{
		private readonly SessionManager _sessionManager;
		private readonly IMapper mapper;
		private readonly IDataRepository<SessionEntity> sessionRepo;

		public SessionService(SessionManager sessionManager,
						IMapper mapper,
						IDataRepository<SessionEntity> sessionRepo)
		{
			_sessionManager = sessionManager;
			this.mapper = mapper;
			this.sessionRepo = sessionRepo;
		}

		public async Task<SessionEntity> CreateUniqueSessionAsync(SessionRequest sessionRequest)
		{
			var session = _sessionManager.CreateUniqueSession(sessionRequest);
			if (session.State == SessionState.Disconnected)
			{
				LoggerManager.Logger.Error($"Cannot connect to: {sessionRequest.Name} {sessionRequest.EndpointUrl}");
				return null;
			}
			var sessionEntity = mapper.Map<SessionEntity>(session);
			await sessionRepo.UpsertAsync(sessionEntity, c => c.Name == sessionRequest.Name);

			return sessionEntity;
		}

		public async Task<SessionEntity> CreateEndpointAsync(SessionRequest sessionRequest)
		{
			var sessionEntity = mapper.Map<SessionEntity>(sessionRequest);
			sessionEntity.SessionId = "";
			await sessionRepo.UpsertAsync(sessionEntity, c => c.Name == sessionRequest.Name);

			return sessionEntity;
		}

		public IEnumerable<SessionEntity> GetActiveSessions()
		{
			var opcUaSessions = _sessionManager.GetSessionList();
			var result = mapper.Map<IEnumerable<SessionEntity>>(opcUaSessions);
			return result;
		}

		public async Task<SessionEntity> FindSessionAsync(int id)
		{
			var result = await sessionRepo.FindAsync(c => c.Id == id);
			return result;
		}

		public async Task DeleteSessionAsync(string sessionName)
		{
			var sessionToDelete = await sessionRepo.FindAsync(c => c.Name == sessionName);
			if (sessionToDelete == null) return;
			_sessionManager.RemoveSession(sessionToDelete.SessionId);
			await sessionRepo.DeleteAsync(sessionToDelete);
		}

		public SessionEntity GetSession(string sessionId)
		{
			_sessionManager.TryGetSession(sessionId, out var session);
			var sessionEntity = mapper.Map<SessionEntity>(session);
			return sessionEntity;
		}

		public async Task UpdateEndpointAsync(SessionEntity request)
		{
			await _sessionManager.DisconnectCurrentAsync();
			await sessionRepo.UpsertAsync(request, c => c.Id == request.Id);
		}

		public async Task<IEnumerable<SessionEntity>> GetSavedSessionsAsync()
		{
			return await sessionRepo.ListAllAsync();
		}

		public async Task Disconnect(SessionRequest request)
		{
			_sessionManager.TryGetSession(request.SessionId, out var session);

			await _sessionManager.DisconnectAsync(session);
		}
	}
}