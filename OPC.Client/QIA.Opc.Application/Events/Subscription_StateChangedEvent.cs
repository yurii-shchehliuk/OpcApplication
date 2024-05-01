namespace QIA.Opc.Application.Events;

using MediatR;
using QIA.Opc.Domain.Entities;

public class Subscription_StateChangedEvent : NotificationData, INotification
{
}
