using MediatR;
using Reports.Application.UseCases.Queries;
using Reports.Domain;
using Reports.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.Application.UseCases.Handlers.QueryHandlers
{
    public class GetAllPostsHandler : IRequestHandler<GetAllPostsQuery, IEnumerable<Post>>
    {

        private readonly ReportDbContext dbContext;

        public GetAllPostsHandler(ReportDbContext dbContext)
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
