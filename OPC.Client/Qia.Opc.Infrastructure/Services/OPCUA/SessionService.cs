using AutoMapper;
using Microsoft.AspNetCore.SignalR;
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

		public async Task<SessionEntity> CreateUniqueSession(SessionDTO endpointUrl)
		{
			var session = await _sessionManager.CreateUniqueSession(endpointUrl);
			var sessionEntity = mapper.Map<SessionEntity>(session);
			await sessionRepo.UpsertAsync(sessionEntity, c => c.Name == endpointUrl.Name);

			return sessionEntity;
		}
		public async Task<IEnumerable<Domain.Entities.SessionEntity>> GetSessionList()
		{
			var result = await sessionRepo.ListAllAsync();
			return result;
		}

		public async Task<bool> DeleteSession(string sessionName)
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
	}
}