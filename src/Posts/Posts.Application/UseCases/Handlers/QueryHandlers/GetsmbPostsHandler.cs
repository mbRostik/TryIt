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
    public class GetsmbPostsHandler : IRequestHandler<GetsmbPostsQuery, List<GiveProfilePostsDTO>>
    {

        private readonly PostDbContext _dbContext;
        private readonly IMapperService _mapper;
        private readonly Serilog.ILogger logger;

        public GetsmbPostsHandler(PostDbContext dbContext, IMapperService mapper, Serilog.ILogger logger)
        {
            this._dbContext = dbContext;
            _mapper = mapper;
            this.logger = logger;
        }

        public async Task<List<GiveProfilePostsDTO>> Handle(GetsmbPostsQuery request, CancellationToken cancellationToken)
        {
            logger.Information("Handling GetsmbPostsQuery for UserId: {UserId}", request.id);

            try
            {
                var result = _dbContext.Posts
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

                logger.Information("Successfully handled GetsmbPostsQuery for UserId: {UserId}, returning {Count} posts", request.id, result.Count);

                return result;
            }

            catch (Exception ex)
            {
                logger.Error(ex, "Error handling GetsmbPostsQuery for UserId: {UserId}", request.id);
                throw; 
            }
        }


    }
}
