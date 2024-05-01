namespace QIA.Opc.Domain.Entities;

using Qia.Opc.Domain.Entities.Enums;

public class NotificationData
{
    public string SessionId { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public LogCategory LogCategory { get; set; } = LogCategory.Info;
}
