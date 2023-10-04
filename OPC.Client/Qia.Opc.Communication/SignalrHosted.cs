using Microsoft.Extensions.Hosting;
using Qia.Opc.Infrastrucutre.Services.Communication;

namespace Qia.Opc.Communication
{
	public class SignalrHosted : BackgroundService
	{
		private readonly SignalRService signalRService;

		public SignalrHosted(SignalRService signalRService)
		{
			this.signalRService = signalRService;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{

			var result = await signalRService.StartConnectionAsync();
			if (!result)
			{
				return;
			}

			var exitEvent = new ManualResetEventSlim();
			Console.CancelKeyPress += (sender, eventArgs) =>
			{
				eventArgs.Cancel = true;
				exitEvent.Set();
			};

			// Wait until Ctrl+C is pressed
			exitEvent.Wait();

			await signalRService.StopConnection();

		}
	}
}
