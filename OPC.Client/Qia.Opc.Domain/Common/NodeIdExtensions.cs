using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qia.Opc.Domain.Common
{
	public static class NodeIdExtensions
	{
		public static bool TryParseNodeId(this string nodeIdString, out NodeId nodeId)
		{
			try
			{
				if (string.IsNullOrEmpty(nodeIdString))
				{
					nodeId = null;
					return false;
				}
				nodeId = new NodeId(nodeIdString);
				return true;
			}
			catch
			{
				nodeId = null;
				return false;
			}
		}
	}

}
