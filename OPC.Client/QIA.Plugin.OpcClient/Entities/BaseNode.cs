using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace QIA.Plugin.OpcClient.Entities
{
	/// <summary>
	/// JSON node model
	/// </summary>
	public class BaseNode : BaseEntity
	{
		//[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public string NodeId { get; set; }
		public string Name { get; set; }
		public uint? MSecs { get; set; }
		public uint? Range { get; set; }
		public string Group { get; set; }
	}

	/// <summary>
	/// outcome node
	/// </summary>
	public class NodeData : BaseEntity
	{
		public NodeData()
		{

		}
		//TODO: replace with automapper
		public NodeData(BaseNode nodeDto, string valueNew = null)
		{
			NodeId = nodeDto.NodeId;
			Name = nodeDto.Name;
			Value = valueNew;
			MSecs = string.IsNullOrEmpty(valueNew) ? null : nodeDto.MSecs;
			Range = string.IsNullOrEmpty(valueNew) ? null : nodeDto.Range;
			StoreTime = string.IsNullOrEmpty(valueNew) ? null : DateTime.UtcNow;
		}
		public string NodeId { get; set; }
		public string Name { get; set; }
		public uint? MSecs { get; set; }
		public uint? Range { get; set; }
		public string Group { get; set; }
		public DateTime? StoreTime { get; set; }
		public string Value { get; set; }
	}

	/// <summary>
	/// income node
	/// </summary>
	public class NewNode : BaseNode
	{
		public MonitorAction Action { get; set; }
	}

	public enum MonitorAction
	{
		Monitor,
		Unmonitor
	}

	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum NodeType
	{
		Object,
		Method,
		Subscription,
		Value
	}
}
