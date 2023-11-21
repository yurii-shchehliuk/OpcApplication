using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QIA.Opc.Domain.Request
{
	/// <summary>
	/// Requested subscription
	/// </summary>
	public class SubscriptionParameters
	{
		public string DisplayName { get; set; } = string.Empty;
		public int PublishingInterval { get; set; } = 1000;
		public uint KeepAliveCount { get; set; } = 10;
		public uint LifetimeCount { get; set; } = 1000;
		public uint MaxNotificationsPerPublish { get; set; } = 0;
		public byte Priority { get; set; } = 255;
		public bool PublishingEnabled { get; set; } = false;
	}
}
