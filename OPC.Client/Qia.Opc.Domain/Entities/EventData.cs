using Qia.Opc.Domain.Entities.Enums;

namespace QIA.Opc.Domain.Entities
{
	public class EventData
	{
		public string Title { get; set; }
		public string Message { get; set; }
		public LogCategory LogCategory { get; set; } = LogCategory.Info;
	}
}
