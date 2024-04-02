using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Userforchat;
using Users.Application.Contracts.DTOs;
using Users.Application.Contracts.Interfaces;
using Users.Domain.Entities;

namespace Users.Infrastructure.Services
{
    public class MapperService : IMapperService
    {
        public IMapper Mapper_ChangeUserProfileToUserDTO()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ChangeProfileInformationDTO, User>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                    .ForMember(dest => dest.NickName, opt => opt.MapFrom(src => src.NickName))
                    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                    .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                    .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.Photo))
                    .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio))
                    .ForMember(dest => dest.IsPrivate, opt => opt.MapFrom(src => src.IsPrivate))
                    .ForMember(dest => dest.SexId, opt => opt.MapFrom(src =>
                         sexStringToIdMapping.ContainsKey(src.SexId) ? sexStringToIdMapping[src.SexId] : 3))
                    .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth));
            });
            var mapper = new Mapper(configuration);
            return mapper;

        }

        public IMapper Mapper_UserToUserProfileDTO()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserProfileDTO>()
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                    .ForMember(dest => dest.NickName, opt => opt.MapFrom(src => src.NickName))
                    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                    .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                    .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.Photo))
                    .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio))
                    .ForMember(dest => dest.IsPrivate, opt => opt.MapFrom(src => src.IsPrivate))
                    .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth));
            });
            var mapper = new Mapper(configuration);
            return mapper;
        }

        public IMapper Mapper_UserToUserChatProfileDTO()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserChatProfileDTO>()
                    .ForMember(dest => dest.NickName, opt => opt.MapFrom(src => src.NickName))
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.Photo));
            });
            var mapper = new Mapper(configuration);
            return mapper;
        }

        public IMapper Mapper_UserChatProfileToGiveUserForChat()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserChatProfileDTO, GiveUserForChat>()
                    .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.NickName, opt => opt.MapFrom(src => src.NickName))
                    .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => Google.Protobuf.ByteString.CopyFrom(src.Photo ?? new byte[] { })));
            });
            var mapper = new Mapper(configuration);
            return mapper;
        }


        Dictionary<string, int> sexStringToIdMapping = new Dictionary<string, int>
        {
            {"Man", 1}, 
            {"Woman", 2}, 
            {"UnIdentify", 3} 
        };
    }
}
