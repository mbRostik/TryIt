using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Posts.Application.Contracts.DTOs;
using Posts.Application.Contracts.Interfaces;
using Posts.Application.UseCases.Queries;
using Posts.Domain.Entities;
using Posts.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Posts.Application.UseCases.Handlers.QueryHandlers
{
    public class GetUserPostsHandler : IRequestHandler<GetUserPostsQuery, List<GiveProfilePostsDTO>>
    {

        private readonly PostDbContext _dbContext;
        private readonly IMapperService _mapper;

        public GetUserPostsHandler(PostDbContext dbContext, IMapperService mapper)
        {
            this._dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<List<GiveProfilePostsDTO>> Handle(GetUserPostsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var postsWithFiles = _dbContext.Posts
                 .AsNoTracking()
                 .Where(x => x.UserId == request.id)
                 .Include(p => p.Files)
                 .ToList();

                var mapper = _mapper.InitializeAutomapper_Post_To_GiveProfilePostDTO();
                List<GiveProfilePostsDTO> result = postsWithFiles.Select(post => mapper.Map<GiveProfilePostsDTO>(post)).ToList();

                return result;
            }
            
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }


    }
}
