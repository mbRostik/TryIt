using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Application.UseCases.Queries;
using Users.Domain.Entities;
using Users.Infrastructure.Data;

namespace Users.Application.UseCases.Handlers.QueryHandlers
{
    public class GetAllPostsHandler : IRequestHandler<GetAllPostsQuery, IEnumerable<Post>>
    {
        public readonly Serilog.ILogger logger;

        private readonly UserDbContext dbContext;

        public GetAllPostsHandler(UserDbContext dbContext, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task<IEnumerable<Post>> Handle(GetAllPostsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                logger.Information("Handling GetAllPostsQuery");

                var result = dbContext.Posts.AsNoTracking().ToList();

                if (result.Any())
                {
                    logger.Information("Successfully retrieved {Count} posts.", result.Count);
                }
                else
                {
                    logger.Information("No posts found.");
                }

                return result;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred while handling GetAllPostsQuery");
                throw; 
            }
        }


    }
}