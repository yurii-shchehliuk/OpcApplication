using Qia.Opc.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qia.Opc.Domain.Common
{
	public class EventData
	{
		public string Title { get; set; }
		public string Message { get; set; }
		public LogCategory LogCategory { get; set; } = LogCategory.Info;
	}
}
