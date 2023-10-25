using Microsoft.Extensions.Hosting;
using Qia.Opc.Domain.Common;
using Qia.Opc.Infrastrucutre.Services.Communication;
using Qia.Opc.Infrastrucutre.Services.OPCUA;
using Qia.Opc.OPCUA.Connector.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qia.Opc.Infrastrucutre.ServicesHosted
{
	public class EventHandlerService : IHostedService
	{
		private readonly SignalRService signalRService;
		private readonly SessionManager sessionManager;
		private readonly SubscriptionManager subscriptionManager;

		public EventHandlerService(SignalRService signalRService, SessionManager sessionManager, SubscriptionManager subscriptionManager)
		{
			this.signalRService = signalRService;
			this.sessionManager = sessionManager;
			this.subscriptionManager = subscriptionManager;
		}

		private async void Manager_EventMessage(object sender, EventData e)
		{
			await SendEventAsync(e);
		}

		public async Task SendEventAsync(EventData e)
		{
			await signalRService.SendEventMessageAsync(e,
			sessionManager.CurrentSession.Name);
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			sessionManager.EventMessage += Manager_EventMessage;
			subscriptionManager.EventMessage += Manager_EventMessage;
			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			sessionManager.EventMessage -= Manager_EventMessage;
			subscriptionManager.EventMessage -= Manager_EventMessage;
			return Task.CompletedTask;
		}
	}
}
