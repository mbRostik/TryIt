using MediatR;
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
    public class GetAllPostsHandler : IRequestHandler<GetAllPostsQuery, IEnumerable<Post>>
    {

        private readonly PostDbContext dbContext;
        private readonly Serilog.ILogger logger;

        public GetAllPostsHandler(PostDbContext dbContext, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task<IEnumerable<Post>> Handle(GetAllPostsQuery request, CancellationToken cancellationToken)
        {
            logger.Information("Handling GetAllPostsQuery");
            var result = dbContext.Posts.ToList();
            return result;
        }


    }
}
