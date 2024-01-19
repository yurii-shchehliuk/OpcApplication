using MediatR;
using Qia.Opc.OPCUA.Connector.Managers;
using QIA.Opc.Domain.Common;
using QIA.Opc.Domain.Entities;
using QIA.Opc.Infrastructure.Services.Communication;

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
				LoggerManager.Logger.Information($"Info: {0}", notification.EventData.Message);

				//await signalRService.SendEventMessageAsync(notification.EventData,
				//	sessionManager.CurrentSession.Name ?? "All");
			}
			catch (Exception ex)
			{
				LoggerManager.Logger.Error($"Cannot process the event: {0}", ex);
			}
		}
	}
}
