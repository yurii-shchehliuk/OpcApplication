using MediatR;
using Qia.Opc.Domain.Common;
using Qia.Opc.Domain.Core;
using Qia.Opc.OPCUA.Connector.Managers;
using QIA.Opc.Infrastructure.Services.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qia.Opc.Infrastructure.Application
{
	public class EventMediatorCommand : INotification
	{
		public EventMediatorCommand(EventData eventData)
		{
			this.EventData = eventData;
		}
		public EventData EventData { get; set; }
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
			try
			{
				await signalRService.SendEventMessageAsync(notification.EventData,
					sessionManager.CurrentSession.Name ?? "All");
			}
			catch (Exception ex)
			{
				LoggerManager.Logger.Error($"Cannot process the event: {0}", ex);
			}
		}
	}
}
