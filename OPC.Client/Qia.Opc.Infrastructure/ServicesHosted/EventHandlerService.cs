using MediatR;
using Microsoft.Extensions.Hosting;
using Qia.Opc.Domain.Common;
using Qia.Opc.Infrastructure.Application;
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
	/// <summary>
	/// OPCUA events handler
	/// </summary>
	public class EventHandlerService : IHostedService
	{
		private readonly SessionManager sessionManager;
		private readonly SubscriptionManager subscriptionManager;
		private readonly IMediator mediator;

		public EventHandlerService(SessionManager sessionManager, SubscriptionManager subscriptionManager, IMediator mediator)
		{
			this.mediator = mediator;
			this.sessionManager = sessionManager;
			this.subscriptionManager = subscriptionManager;
		}

		private async void Manager_EventMessage(object sender, EventData e)
		{
			await mediator.Publish(new EventMediatorCommand(e));
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
