using AutoMapper;
using Posts.Application.Contracts.DTOs;
using Posts.Application.Contracts.Interfaces;
using Posts.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Posts.Infrastructure.Services
{
    public class MapperService:IMapperService
    {
        public IMapper InitializeAutomapper_Post_To_GiveProfilePostDTO()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Post, GiveProfilePostsDTO>()
                     .ForMember(dest => dest.files, act => act.MapFrom(src => src.Files.ToList()))
                     .ForMember(dest => dest.Title, act => act.MapFrom(src => src.Title))
                     .ForMember(dest => dest.Content, act => act.MapFrom(src => src.Content));
            });
            var mapper = new Mapper(config);
            return mapper;
        }

        public IMapper InitializeAutomapper_CreatePostDTO_To_Post()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreatePostDTO, Post>()
                   .ForMember(dest => dest.Title, act => act.MapFrom(src => src.Title))
                   .ForMember(dest => dest.Content, act => act.MapFrom(src => src.Content))
                   .ForMember(dest => dest.UserId, act => act.MapFrom(src => src.UserId))
                   .ForMember(dest => dest.Date, act => act.MapFrom(src => DateTime.UtcNow))
                   .ForMember(dest => dest.Files, act => act.MapFrom(src =>
                        src.Files.Select(fileModel => new PFile
                        {
                            Name = fileModel.Name, 
                            file = Convert.FromBase64String(fileModel.Content), 
                            Date = DateTime.UtcNow
                        }).ToList()));
            });
            return config.CreateMapper();
        }
    }
}
