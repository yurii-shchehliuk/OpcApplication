using Microsoft.Extensions.Hosting;

namespace QIA.Opc.Infrastructure.ServicesHosted
{
	public class ServerStatusChecker : IHostedService
	{
		public ServerStatusChecker()
		{
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}


		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}
