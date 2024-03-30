using MediatR;
using Microsoft.EntityFrameworkCore;
using Posts.Application.Contracts.DTOs;
using Posts.Application.Contracts.Interfaces;
using Posts.Application.UseCases.Queries;
using Posts.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Posts.Application.UseCases.Handlers.QueryHandlers
{
    internal class GetsmbPostsHandler : IRequestHandler<GetsmbPostsQuery, List<GiveProfilePostsDTO>>
    {

        private readonly PostDbContext _dbContext;
        private readonly IMapperService _mapper;

        public GetsmbPostsHandler(PostDbContext dbContext, IMapperService mapper)
        {
            this._dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<List<GiveProfilePostsDTO>> Handle(GetsmbPostsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var postsWithFiles = _dbContext.Posts
                    .AsNoTracking()
                    .Where(x => x.UserId == request.id)
                    .Select(p => new GiveProfilePostsDTO
                    {
                        Title = p.Title,
                        Content = p.Content,
                        Date = p.Date,
                        Files = p.Files.Select(f => new GiveFileDTO
                        {
                            Id = f.Id,
                            Name = f.Name,
                            file = f.file,
                            Date = f.Date,
                            PostId = f.PostId
                        }).ToList()
                    })
                    .ToList();

                var mapper = _mapper.InitializeAutomapper_Post_To_GiveProfilePostDTO();
                List<GiveProfilePostsDTO> result = postsWithFiles.Select(post => mapper.Map<GiveProfilePostsDTO>(post)).ToList();

                return result;
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }


    }
}
