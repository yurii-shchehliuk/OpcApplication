using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Qia.Opc.Domain.Core;
using Qia.Opc.Domain.DTO;
using Qia.Opc.Domain.Entities;
using Qia.Opc.Infrastrucutre.Services.Communication;
using Qia.Opc.OPCUA.Connector.Entities;
using Qia.Opc.OPCUA.Connector.Managers;
using Qia.Opc.Persistence.Repository;

namespace Qia.Opc.Infrastrucutre.Services.OPCUA
{
	/// <summary>
	/// session management
	/// </summary>
	public class SessionService
	{
		private readonly SessionManager _sessionManager;
		private readonly IMapper mapper;
		private readonly IDataRepository<SessionEntity> sessionRepo;

		public SessionService(SessionManager sessionManager, IMapper mapper, IDataRepository<SessionEntity> sessionRepo)
		{
			_sessionManager = sessionManager;
			this.mapper = mapper;
			this.sessionRepo = sessionRepo;
		}

		public async Task<SessionEntity> CreateUniqueSessionAsync(SessionDTO sessionDto)
		{
			var session = _sessionManager.CreateUniqueSession(sessionDto);
			if (session.State == Domain.Entities.Enums.SessionState.Disconnected)
			{
				LoggerManager.Logger.Error($"Cannot connect to: {sessionDto.Name} {sessionDto.EndpointUrl}");
				return null;
			}
			var sessionEntity = mapper.Map<SessionEntity>(session);
			await sessionRepo.UpsertAsync(sessionEntity, c => c.Name == sessionDto.Name);

			return sessionEntity;
		}

		public async Task<SessionEntity> CreateEndpointAsync(SessionDTO sessionDto)
		{
			var sessionEntity = mapper.Map<SessionEntity>(sessionDto);
			sessionEntity.SessionId = "";
			await sessionRepo.UpsertAsync(sessionEntity, c => c.Name == sessionDto.Name);

			return sessionEntity;
		}

		public async Task<IEnumerable<Domain.Entities.SessionEntity>> GetSessionListAsync()
		{
			var result = await sessionRepo.ListAllAsync();
			return result;
		}

		public async Task<Domain.Entities.SessionEntity> FindSessionAsync(int id)
		{
			var result = await sessionRepo.FindAsync(c => c.Id == id);
			return result;
		}

		public async Task<bool> DeleteSessionAsync(string sessionName)
		{
			var sessionToDelete = await sessionRepo.FindAsync(c => c.Name == sessionName);
			if (sessionToDelete == null) return false;
			await sessionRepo.DeleteAsync(sessionToDelete);
			return _sessionManager.RemoveSession(sessionToDelete.SessionId);
		}

		public SessionEntity GetSession(string sessionId)
		{
			_sessionManager.TryGetSession(sessionId, out var session);
			var sessionEntity = mapper.Map<SessionEntity>(session);
			return sessionEntity;
		}

		public SessionEntity GetCurrentSession()
		{
			var session = _sessionManager.CurrentSession;
			var sessionEntity = mapper.Map<SessionEntity>(session);
			return sessionEntity;
		}
	}
}