namespace QIA.Opc.Infrastructure.Events;

using MediatR;
using QIA.Opc.Application.Events;
using QIA.Opc.Infrastructure.Application;
using QIA.Opc.Infrastructure.Services.Communication;

public class SessionStateEventHandler : INotificationHandler<SessionStateEvent>
{
    private readonly SignalRService _signalRService;
    public SessionStateEventHandler(SignalRService signalRService)
    {
        _signalRService = signalRService;
    }
    public async Task Handle(SessionStateEvent notification, CancellationToken cancellationToken)
    {
        LoggerManager.Logger.Information(notification.Message + notification.Title);
        await _signalRService.SendNotification(notification, notification.SessionId);
    }
}
