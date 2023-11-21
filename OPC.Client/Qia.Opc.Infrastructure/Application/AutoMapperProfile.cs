using AutoMapper;
using Opc.Ua;
using Opc.Ua.Client;
using Qia.Opc.Domain.Entities;
using Qia.Opc.OPCUA.Connector.Entities;
using QIA.Opc.Domain.Request;
using QIA.Opc.Domain.Response;

namespace QIA.Opc.Infrastructure.Application
{
	public class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		{
			CreateMap<OPCUASession, SessionEntity>()
			.ForMember(dest => dest.SessionNodeId, opt => opt.MapFrom(src => src.Session.SessionId.ToString()))
			.ForMember(dest => dest.SessionId, opt => opt.MapFrom(src => src.SessionId.ToString()));

			CreateMap<SessionRequest, SessionEntity>();

			CreateMap<ReferenceDescription, NodeReferenceEntity>()
					  .ForMember(dest => dest.NodeId, opt => opt.MapFrom(src => src.NodeId.ToString()))
					  .ForMember(dest => dest.NodeClass, opt => opt.MapFrom(src => src.NodeClass))
					  .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName.ToString()));

			CreateMap<Subscription, Domain.Response.SubscriptionResponce>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.SessionNodeId, opt => opt.MapFrom(src => src.Session.SessionId))
				.ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
				.ForMember(dest => dest.PublishInterval, opt => opt.MapFrom(src => src.PublishingInterval))
				.ForMember(dest => dest.ItemsCount, opt => opt.MapFrom(src => src.MonitoredItemCount))
				.ForMember(dest => dest.SequenceNumber, opt => opt.MapFrom(src => src.SequenceNumber))
				.ForMember(dest => dest.MonitoringItems, opt => opt.MapFrom(src => src.MonitoredItems));

			CreateMap<MonitoredItem, MonitoredItemResponse>()
				.ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
				.ForMember(dest => dest.SamplingInterval, opt => opt.MapFrom(src => src.SamplingInterval))
				.ForMember(dest => dest.QueueSize, opt => opt.MapFrom(src => src.QueueSize))
				.ForMember(dest => dest.StartNodeId, opt => opt.MapFrom(src => src.StartNodeId.ToString()));
		}
	}
}
