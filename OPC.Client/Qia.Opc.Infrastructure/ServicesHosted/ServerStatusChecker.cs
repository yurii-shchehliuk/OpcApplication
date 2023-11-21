using Microsoft.Extensions.Hosting;
using QIA.Opc.Infrastructure.Services.OPCUA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
