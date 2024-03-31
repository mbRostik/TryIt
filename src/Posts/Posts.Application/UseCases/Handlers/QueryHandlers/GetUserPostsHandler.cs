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
        private readonly Serilog.ILogger logger;

        public GetUserPostsHandler(PostDbContext dbContext, IMapperService mapper, Serilog.ILogger logger)
        {
            this._dbContext = dbContext;
            _mapper = mapper;
            this.logger = logger;
        }

        public async Task<List<GiveProfilePostsDTO>> Handle(GetUserPostsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                logger.Information("Handling GetUserPostsQuery for UserId: {UserId}", request.id);

                var result = _dbContext.Posts
                    .AsNoTracking()
                    .Where(x => x.UserId == request.id)
                    .Select(p => new GiveProfilePostsDTO
                    {
                        Id = p.Id,
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

                logger.Information("Successfully handled GetUserPostsQuery for UserId: {UserId}. Found {Count} posts.", request.id, result.Count);

                return result;
            }

            catch (Exception ex)
            {
                logger.Error(ex, "Error handling GetUserPostsQuery for UserId: {UserId}", request.id);
                throw;
            }
        }


    }
}
