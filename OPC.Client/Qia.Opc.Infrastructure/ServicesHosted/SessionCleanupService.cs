using Microsoft.Extensions.Hosting;
using Qia.Opc.Infrastrucutre.Services.OPCUA;

namespace Qia.Opc.Infrastrucutre.ServicesHosted
{
	public class SessionCleanupService : IHostedService, IDisposable
	{
		private readonly CleanerService _opcuaService;
		private Timer _timer;

		public SessionCleanupService(CleanerService opcuaService)
		{
			_opcuaService = opcuaService;
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			_timer = new Timer(CleanupSessions, null, TimeSpan.Zero, TimeSpan.FromHours(1));  // Cleanup every hour
			return Task.CompletedTask;
		}

		private void CleanupSessions(object state)
		{
			_opcuaService.CleanupStaleSessions();
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_timer?.Change(Timeout.Infinite, 0);
			return Task.CompletedTask;
		}

		public void Dispose()
		{
			_timer?.Dispose();
		}
	}

}
