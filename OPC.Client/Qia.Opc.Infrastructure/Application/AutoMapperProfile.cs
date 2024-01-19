using AutoMapper;
using Opc.Ua;
using Opc.Ua.Client;
using Qia.Opc.Domain.Entities;
using Qia.Opc.OPCUA.Connector.Entities;
using QIA.Opc.Domain.Common;
using QIA.Opc.Domain.Entities;
using QIA.Opc.Domain.Requests;
using QIA.Opc.Domain.Responses;

namespace QIA.Opc.Infrastructure.Application
{
	public class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		{
			MapSessions();

			MapSubscriptions();

			MapMonitoredItems();
		}

		private void MapSessions()
		{
			CreateMap<OPCUASession, SessionResponse>()
			  .ForMember(dest => dest.Guid, opt => opt.MapFrom(src => src.Guid))
			  .ForMember(dest => dest.SessionNodeId, opt => opt.MapFrom(src => src.SessionNodeId))
			  .ForMember(dest => dest.Guid, opt => opt.MapFrom(src => src.Guid))
			  .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
			  .ForMember(dest => dest.EndpointUrl, opt => opt.MapFrom(src => src.EndpointUrl));

			CreateMap<SessionEntity, SessionResponse>()
			  .ForMember(dest => dest.Guid, opt => opt.MapFrom(src => src.Guid));
		}

		private void MapSubscriptions()
		{
			// on create to save to db
			CreateMap<Subscription, SubscriptionConfig>()
					.ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
					.ForMember(dest => dest.PublishingInterval, opt => opt.MapFrom(src => src.PublishingInterval))
					.ForMember(dest => dest.MonitoredItemsConfig, opt => opt.MapFrom(src => src.MonitoredItems));

			// on create to send value
			CreateMap<Subscription, SubscriptionValue>()
				.ForMember(dest => dest.OpcUaId, opt => opt.MapFrom(src => src.Id.ToString()))
				.ForMember(dest => dest.SessionNodeId, opt => opt.MapFrom(src => src.Session.SessionId.ToString()))
				.ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
				.ForMember(dest => dest.PublishingEnabled, opt => opt.MapFrom(src => src.PublishingEnabled))
				.ForMember(dest => dest.PublishingInterval, opt => opt.MapFrom(src => src.PublishingInterval))
				.ForMember(dest => dest.ItemsCount, opt => opt.MapFrom(src => src.MonitoredItemCount))
				.ForMember(dest => dest.SequenceNumber, opt => opt.MapFrom(src => src.SequenceNumber))
				.ForMember(dest => dest.MonitoredItems, opt => opt.MapFrom(src => src.MonitoredItems));

			// on get from database
			CreateMap<SubscriptionConfig, SubscriptionValue>()
				.ForMember(dest => dest.Guid, opt => opt.MapFrom(src => src.Guid))
				.ForMember(dest => dest.OpcUaId, opt => opt.MapFrom(src => 0))
				.ForMember(dest => dest.MonitoredItems, opt => opt.MapFrom(src => src.MonitoredItemsConfig))
				.ForMember(dest => dest.PublishingEnabled, opt => opt.MapFrom(src => false))
				.ForMember(dest => dest.ItemsCount, opt => opt.MapFrom(src => src.MonitoredItemsConfig.Count()));

			// on get from memory
			CreateMap<OPCUASubscription, SubscriptionValue>()
				.ForMember(dest => dest.OpcUaId, opt => opt.MapFrom(src => src.Subscription.Id))
				.ForMember(dest => dest.Guid, opt => opt.MapFrom(src => src.Guid))
				.ForMember(dest => dest.MonitoredItems, opt => opt.MapFrom(src => src.Subscription.MonitoredItems))
				.ForMember(dest => dest.PublishingEnabled, opt => opt.MapFrom(src => src.Subscription.PublishingEnabled))
				.ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Subscription.DisplayName))
				.ForMember(dest => dest.PublishingInterval, opt => opt.MapFrom(src => src.Subscription.PublishingInterval))
				.ForMember(dest => dest.SequenceNumber, opt => opt.MapFrom(src => src.Subscription.SequenceNumber))
				.ForMember(dest => dest.SessionNodeId, opt => opt.MapFrom(src => src.Subscription.Session.SessionId.ToString()))
				.ForMember(dest => dest.ItemsCount, opt => opt.MapFrom(src => src.Subscription.MonitoredItemCount));

			CreateMap<SubscriptionRequest, SubscriptionConfig>()
				.ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
				.ForMember(dest => dest.PublishingInterval, opt => opt.MapFrom(src => src.PublishingInterval));
		}

		private void MapMonitoredItems()
		{
			CreateMap<ReferenceDescription, MonitoredItemConfig>()
						  .ForMember(dest => dest.StartNodeId, opt => opt.MapFrom(src => src.NodeId.ToString()))
						  .ForMember(dest => dest.NodeClass, opt => opt.MapFrom(src => src.NodeClass))
						  .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName.ToString()));

			CreateMap<MonitoredItem, MonitoredItemConfig>()
				.ForMember(dest => dest.OpcUaId, opt => opt.MapFrom(src => OpcIdMapExtension(src)))
				.ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
				.ForMember(dest => dest.SamplingInterval, opt => opt.MapFrom(src => src.SamplingInterval))
				.ForMember(dest => dest.QueueSize, opt => opt.MapFrom(src => src.QueueSize))
				.ForMember(dest => dest.StartNodeId, opt => opt.MapFrom(src => src.StartNodeId.ToString()))
				.ForMember(dest => dest.IndexRange, opt => opt.MapFrom(src => src.IndexRange));

			CreateMap<MonitoredItem, MonitoredItemValue>()
				.ForMember(dest => dest.ServerId, opt => opt.MapFrom(src => OpcIdMapExtension(src)))
				.ForMember(dest => dest.SubscriptionOpcId, opt => opt.MapFrom(src => src.Subscription.Id))
				.ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
				.ForMember(dest => dest.SamplingInterval, opt => opt.MapFrom(src => src.SamplingInterval))
				.ForMember(dest => dest.QueueSize, opt => opt.MapFrom(src => src.QueueSize))
				.ForMember(dest => dest.Value, opt => opt.MapFrom(src => ValueMapExtension(src)))
				.ForMember(dest => dest.StartNodeId, opt => opt.MapFrom(src => src.StartNodeId.ToString()));

			CreateMap<MonitoredItemConfig, MonitoredItemValue>()
				.ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
				.ForMember(dest => dest.SamplingInterval, opt => opt.MapFrom(src => src.SamplingInterval))
				.ForMember(dest => dest.QueueSize, opt => opt.MapFrom(src => src.QueueSize))
				.ForMember(dest => dest.StartNodeId, opt => opt.MapFrom(src => src.StartNodeId.ToString()));
		}

		private static string ValueMapExtension(MonitoredItem item)
		{
			try
			{
				var result = item.DequeueValues();
				if (result.Count == 0)
				{
					if (item.LastValue == null)
						return "0";

					var data = (item.LastValue as dynamic).Value.Value.ToString();
					return data ?? "0";
				}
				return result[0].ToString();
			}
			catch (Exception ex)
			{
				LoggerManager.Logger.Warning("Value map error: {0}", ex);
				return "0";
			}
		}

		private static int OpcIdMapExtension(MonitoredItem item)
		{
			try
			{
				int.TryParse(item.ServerId.ToString(), out int id);
				return id;
			}
			catch (Exception ex)
			{
				LoggerManager.Logger.Warning("Value map error: {0}", ex);
				return 0;
			}
		}
	}
}
