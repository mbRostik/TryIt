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

        public GetAllPostsHandler(PostDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<Post>> Handle(GetAllPostsQuery request, CancellationToken cancellationToken)
        {
            var result = dbContext.Posts.ToList();
            return result;
        }


    }
}
