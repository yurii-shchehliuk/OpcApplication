using AutoMapper;
using Opc.Ua;
using Opc.Ua.Client;
using Qia.Opc.Domain.DTO;
using Qia.Opc.Domain.Entities;
using Qia.Opc.OPCUA.Connector.Entities;

namespace Qia.Opc.Infrastrucutre.Application
{
	public class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		{
			CreateMap<OPCUASession, SessionEntity>();
			CreateMap<SessionDTO, SessionEntity>();

			CreateMap<ReferenceDescription, NodeReferenceEntity>()
					  .ForMember(dest => dest.NodeId, opt => opt.MapFrom(src => src.NodeId.ToString()))
					  .ForMember(dest => dest.NodeClass, opt => opt.MapFrom(src => src.NodeClass))
					  .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName.ToString()));

			CreateMap<Subscription, SubscriptionDTO>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName));
		}
	}
}
