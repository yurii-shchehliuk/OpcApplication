using AutoMapper;
using QIA.Plugin.OpcClient.DTO;
using QIA.Plugin.OpcClient.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QIA.Plugin.OpcClient.Core
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<PluginConfigurationDTO, AppSettings>();
            CreateMap<StandaloneConfigurationDTO, AppSettings>();
        }
    }
}
