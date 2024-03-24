using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Chats.Application.Contracts.DTOs;
using Chats.Application.Contracts.Interfaces;
using Chats.Domain.Entities;

namespace Chats.Infrastructure.Services
{
    public class MapperService : IMapperService
    {
        public void Mapper_Message_To_GiveUserChatMessagesDTO(ref IMapper mapper)
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Message, GiveUserChatMessagesDTO>()
                    .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                    .ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => src.SenderId))
                    .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date));
            });
            mapper = configuration.CreateMapper();
        }
    }
}
