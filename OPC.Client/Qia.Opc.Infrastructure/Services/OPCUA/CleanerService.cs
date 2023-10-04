using Qia.Opc.OPCUA.Connector.Managers;

namespace Qia.Opc.Infrastrucutre.Services.OPCUA
{
	public class CleanerService
	{
		private readonly SessionManager sessionManager;

		public CleanerService(SessionManager sessionManager)
		{
			this.sessionManager = sessionManager;
		}

		internal void CleanupStaleSessions()
		{
			sessionManager.CleanupExpiredSessions(TimeSpan.FromHours(1));
		}
	}
}
