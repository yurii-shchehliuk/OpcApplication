using MediatR;
using Qia.Opc.Domain.Common;
using Qia.Opc.Infrastrucutre.Services.Communication;
using Qia.Opc.OPCUA.Connector.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qia.Opc.Infrastructure.Application
{
	public class EventMediatorCommand : INotification
	{
		public EventData EventData { get; set; }
		public EventMediatorCommand(EventData eventData)
		{
			this.EventData = eventData;
		}
	}

	public class EventMediatorHandler : INotificationHandler<EventMediatorCommand>
	{
		private readonly SignalRService signalRService;
		private readonly SessionManager sessionManager;

		public EventMediatorHandler(SignalRService signalRService, SessionManager sessionManager)
		{
			this.signalRService = signalRService;
			this.sessionManager = sessionManager;
		}

		public async Task Handle(EventMediatorCommand notification, CancellationToken cancellationToken)
		{
			await signalRService.SendEventMessageAsync(notification.EventData,
			sessionManager.CurrentSession.Name);
		}
	}
}
